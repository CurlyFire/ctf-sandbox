using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Data;
using ctf_sandbox.Models;

namespace ctf_sandbox.Controllers;

[Authorize]
public class TeamsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public TeamsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Teams
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var teams = await _context.Teams
            .Include(t => t.Owner)
            .Include(t => t.Members)
                .ThenInclude(m => m.User)
            .Where(t => t.OwnerId == currentUser!.Id || t.Members.Any(m => m.UserId == currentUser.Id && !m.IsInvitePending))
            .ToListAsync();
        return View(teams);
    }

    // GET: Teams/Create
    public IActionResult Create()
    {
        return View(new Team());
    }

    // POST: Teams/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Team team)
    {
        // Remove properties that shouldn't be bound
        ModelState.Remove("OwnerId");
        ModelState.Remove("CreatedAt");
        ModelState.Remove("Owner");
        ModelState.Remove("Members");

        if (ModelState.IsValid)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            team.OwnerId = currentUser!.Id;
            team.CreatedAt = DateTime.UtcNow;
            
            _context.Add(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // If we got this far, something failed, redisplay form with validation messages
        return View(team);
    }

    // GET: Teams/Invite/5
    public async Task<IActionResult> Invite(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (team == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (team.OwnerId != currentUser!.Id)
        {
            return Forbid();
        }

        return View(team);
    }

    // POST: Teams/Invite/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Invite(int id, string email)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (team.OwnerId != currentUser!.Id)
        {
            return Forbid();
        }

        var invitedUser = await _userManager.FindByEmailAsync(email);
        if (invitedUser == null)
        {
            ModelState.AddModelError("", "User not found");
            return View(team);
        }

        var existingMember = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == id && tm.UserId == invitedUser.Id);

        if (existingMember != null)
        {
            ModelState.AddModelError("", "User is already a member or has a pending invitation");
            return View(team);
        }

        var teamMember = new TeamMember
        {
            TeamId = id,
            UserId = invitedUser.Id,
            JoinedAt = DateTime.UtcNow,
            IsInvitePending = true
        };

        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Teams/Invitations
    public async Task<IActionResult> Invitations()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var pendingInvitations = await _context.TeamMembers
            .Include(tm => tm.Team)
            .Include(tm => tm.Team.Owner)
            .Where(tm => tm.UserId == currentUser!.Id && tm.IsInvitePending)
            .ToListAsync();

        return View(pendingInvitations);
    }

    // POST: Teams/AcceptInvite/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptInvite(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var invitation = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.Id == id && tm.UserId == currentUser!.Id && tm.IsInvitePending);

        if (invitation == null)
        {
            return NotFound();
        }

        invitation.IsInvitePending = false;
        invitation.JoinedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: Teams/DeclineInvite/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeclineInvite(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var invitation = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.Id == id && tm.UserId == currentUser!.Id && tm.IsInvitePending);

        if (invitation == null)
        {
            return NotFound();
        }

        _context.TeamMembers.Remove(invitation);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Invitations));
    }
}