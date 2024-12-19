using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Data;
using ctf_sandbox.Models;

namespace ctf_sandbox.Controllers;

[Authorize]
public class ChallengesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ChallengesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Challenges
    public async Task<IActionResult> Index()
    {
        var challenges = await _context.Challenges
            .Include(c => c.Creator)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(challenges);
    }

    // GET: Challenges/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var challenge = await _context.Challenges
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challenge == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        // Only allow creators and admins to see the details (with flag)
        if (challenge.CreatorId != currentUser!.Id && !User.IsInRole("Admin"))
        {
            // For regular users, look for an active competition containing this challenge
            var now = DateTime.UtcNow;
            var competitionChallenge = await _context.CompetitionChallenges
                .Include(cc => cc.Competition)
                .FirstOrDefaultAsync(cc => 
                    cc.ChallengeId == id && 
                    cc.Competition.StartDate <= now &&
                    cc.Competition.EndDate >= now);

            if (competitionChallenge != null)
            {
                // If the challenge is in an active competition, redirect to solve view
                return RedirectToAction(nameof(Solve), new { id, competitionId = competitionChallenge.CompetitionId });
            }
            else
            {
                // If not in competition, show a message that they can't access it
                TempData["ErrorMessage"] = "This challenge is not currently available in any active competition.";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(challenge);
    }

    // GET: Challenges/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Challenges/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,Flag,Points,Category,Content")] PlaintextChallenge challenge)
    {
        ModelState.Remove("CreatorId");
        ModelState.Remove("Creator");
        ModelState.Remove("CreatedAt");
        ModelState.Remove("ChallengeType");

        if (ModelState.IsValid)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            challenge.CreatorId = currentUser!.Id;
            challenge.CreatedAt = DateTime.UtcNow;
            challenge.ChallengeType = "Plaintext";

            _context.Add(challenge);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(challenge);
    }

    // GET: Challenges/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var challenge = await _context.Challenges.FindAsync(id);
        if (challenge == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (challenge.CreatorId != currentUser!.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return View(challenge);
    }

    // POST: Challenges/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Flag,Points,Category,Content,ChallengeType")] PlaintextChallenge challenge)
    {
        if (id != challenge.Id)
        {
            return NotFound();
        }

        var existingChallenge = await _context.Challenges.FindAsync(id);
        if (existingChallenge == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (existingChallenge.CreatorId != currentUser!.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        ModelState.Remove("CreatorId");
        ModelState.Remove("Creator");
        ModelState.Remove("CreatedAt");

        if (ModelState.IsValid)
        {
            try
            {
                challenge.CreatorId = existingChallenge.CreatorId;
                challenge.CreatedAt = existingChallenge.CreatedAt;
                _context.Update(challenge);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChallengeExists(challenge.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(challenge);
    }

    // GET: Challenges/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var challenge = await _context.Challenges
            .Include(c => c.Creator)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challenge == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (challenge.CreatorId != currentUser!.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return View(challenge);
    }

    // POST: Challenges/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var challenge = await _context.Challenges.FindAsync(id);
        if (challenge == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (challenge.CreatorId != currentUser!.Id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        _context.Challenges.Remove(challenge);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Challenges/Solve/5
    public async Task<IActionResult> Solve(int? id, int competitionId)
    {
        if (id == null)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;
        var userId = _userManager.GetUserId(User);

        // Verify the challenge is part of the competition and competition is active
        var competitionChallenge = await _context.CompetitionChallenges
            .Include(cc => cc.Challenge)
            .Include(cc => cc.Competition)
            .FirstOrDefaultAsync(cc => 
                cc.ChallengeId == id && 
                cc.CompetitionId == competitionId &&
                cc.Competition.StartDate <= now &&
                cc.Competition.EndDate >= now);

        if (competitionChallenge == null)
        {
            return NotFound("Challenge not found in active competition");
        }

        // Verify user is part of a team in the competition
        var userTeamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId && !tm.IsInvitePending)
            .Select(tm => tm.TeamId)
            .ToListAsync();

        var isInCompetition = await _context.CompetitionTeams
            .AnyAsync(ct => ct.CompetitionId == competitionId && userTeamIds.Contains(ct.TeamId));

        if (!isInCompetition)
        {
            return BadRequest("You must be part of a team in this competition to solve challenges");
        }

        ViewBag.CompetitionId = competitionId;
        return View(competitionChallenge.Challenge);
    }

    // POST: Challenges/SubmitFlag
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitFlag(int id, string flag)
    {
        var userId = _userManager.GetUserId(User);
        var challenge = await _context.Challenges.FindAsync(id);

        if (challenge == null)
        {
            return NotFound();
        }

        // Find active competitions containing this challenge
        var now = DateTime.UtcNow;
        var competitionChallenge = await _context.CompetitionChallenges
            .Include(cc => cc.Competition)
            .Include(cc => cc.Competition.Teams)
            .FirstOrDefaultAsync(cc => 
                cc.ChallengeId == id && 
                cc.Competition.StartDate <= now &&
                cc.Competition.EndDate >= now);

        if (competitionChallenge == null)
        {
            return BadRequest("This challenge is not part of any active competition.");
        }

        // Find the user's team in this competition
        var userTeamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId && !tm.IsInvitePending)
            .Select(tm => tm.TeamId)
            .ToListAsync();

        var competitionTeam = competitionChallenge.Competition.Teams
            .FirstOrDefault(ct => userTeamIds.Contains(ct.TeamId));

        if (competitionTeam == null)
        {
            return BadRequest("You are not part of any team in this competition.");
        }

        if (flag == challenge.Flag)
        {
            // Update team score
            competitionTeam.Score += competitionChallenge.Points;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Correct! Your team earned {competitionChallenge.Points} points!";
            return RedirectToAction(nameof(Solve), new { id, competitionId = competitionChallenge.CompetitionId });
        }

        TempData["ErrorMessage"] = "Incorrect flag. Try again!";
        return RedirectToAction(nameof(Solve), new { id, competitionId = competitionChallenge.CompetitionId });
    }

    private bool ChallengeExists(int id)
    {
        return _context.Challenges.Any(e => e.Id == id);
    }
}