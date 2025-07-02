using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Data;
using ctf_sandbox.Models;

namespace ctf_sandbox.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class CompetitionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CompetitionsController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var competitions = await _context.Competitions
            .Include(c => c.Teams)
            .Include(c => c.Challenges)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return View(competitions);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate")] Competition competition)
    {
        // Validate dates
        if (competition.StartDate < DateTime.UtcNow)
        {
            ModelState.AddModelError("StartDate", "Start date cannot be in the past");
        }
        if (competition.EndDate <= competition.StartDate)
        {
            ModelState.AddModelError("EndDate", "End date must be after start date");
        }

        // Remove navigation properties from validation
        ModelState.Remove("Creator");

        if (ModelState.IsValid)
        {
            competition.CreatorId = _userManager.GetUserId(User);
            competition.CreatedAt = DateTime.UtcNow;

            // Load and set the Creator navigation property
            var creator = await _userManager.FindByIdAsync(competition.CreatorId);
            competition.Creator = creator;

            _context.Add(competition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(competition);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var competition = await _context.Competitions
            .Include(c => c.Teams)
                .ThenInclude(ct => ct.Team)
            .Include(c => c.Challenges)
                .ThenInclude(cc => cc.Challenge)
            .FirstOrDefaultAsync(c => c.Id == id);

        // Remove navigation properties from validation
        ModelState.Remove("Creator");

        if (competition == null)
        {
            return NotFound();
        }

        ViewBag.AvailableTeams = await _context.Teams.ToListAsync();
        ViewBag.AvailableChallenges = await _context.Challenges.ToListAsync();

        return View(competition);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate,CreatorId,CreatedAt")] Competition competition)
    {
        if (id != competition.Id)
        {
            return NotFound();
        }

        // Remove navigation properties from validation
        ModelState.Remove("Creator");

        if (ModelState.IsValid)
        {
            try
            {
                // Get the existing competition to preserve creator info
                var existingCompetition = await _context.Competitions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existingCompetition == null)
                {
                    return NotFound();
                }

                // Preserve the creator info
                competition.CreatorId = existingCompetition.CreatorId;
                
                _context.Update(competition);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetitionExists(competition.Id))
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

        // If we got this far, something failed
        ViewBag.AvailableTeams = await _context.Teams.ToListAsync();
        ViewBag.AvailableChallenges = await _context.Challenges.ToListAsync();
        return View(competition);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTeam(int competitionId, int teamId)
    {
        var competition = await _context.Competitions.FindAsync(competitionId);
        var team = await _context.Teams.FindAsync(teamId);

        if (competition == null || team == null)
        {
            return NotFound();
        }

        var competitionTeam = new CompetitionTeam
        {
            CompetitionId = competitionId,
            TeamId = teamId,
            JoinedAt = DateTime.UtcNow,
            Score = 0
        };

        _context.CompetitionTeams.Add(competitionTeam);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Edit), new { id = competitionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTeam(int competitionId, int teamId)
    {
        var competitionTeam = await _context.CompetitionTeams
            .FirstOrDefaultAsync(ct => ct.CompetitionId == competitionId && ct.TeamId == teamId);

        if (competitionTeam == null)
        {
            return NotFound();
        }

        _context.CompetitionTeams.Remove(competitionTeam);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Edit), new { id = competitionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddChallenge(int competitionId, int challengeId, int points)
    {
        var competition = await _context.Competitions.FindAsync(competitionId);
        var challenge = await _context.Challenges.FindAsync(challengeId);

        if (competition == null || challenge == null)
        {
            return NotFound();
        }

        var competitionChallenge = new CompetitionChallenge
        {
            CompetitionId = competitionId,
            ChallengeId = challengeId,
            AddedAt = DateTime.UtcNow,
            Points = points
        };

        _context.CompetitionChallenges.Add(competitionChallenge);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Edit), new { id = competitionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveChallenge(int competitionId, int challengeId)
    {
        var competitionChallenge = await _context.CompetitionChallenges
            .FirstOrDefaultAsync(cc => cc.CompetitionId == competitionId && cc.ChallengeId == challengeId);

        if (competitionChallenge == null)
        {
            return NotFound();
        }

        _context.CompetitionChallenges.Remove(competitionChallenge);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Edit), new { id = competitionId });
    }

    private bool CompetitionExists(int id)
    {
        return _context.Competitions.Any(e => e.Id == id);
    }
}