using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using ctf_sandbox.Data;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Services;

namespace ctf_sandbox.Areas.CTF.Controllers;

[Area("CTF")]
[Authorize]
public class TeamsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly TimeProvider _timeProvider;
    private readonly ITeamsService _teamsService;

    public TeamsController(
        ApplicationDbContext context, 
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        TimeProvider timeProvider,
        ITeamsService teamsService)
    {
        _context = context;
        _userManager = userManager;
        _emailSender = emailSender;
        _timeProvider = timeProvider;
        _teamsService = teamsService;
    }

    // GET: Teams
    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var teams = await _teamsService.GetTeamsForUserAsync(currentUser!.Id);
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
            var (success, errorMessage, _) = await _teamsService.CreateTeamAsync(currentUser!.Id, team.Name, team.Description);

            if (!success)
            {
                ModelState.AddModelError("", errorMessage ?? "An error occurred");
                return View(team);
            }

            return RedirectToAction(nameof(Index));
        }

        // If we got this far, something failed, redisplay form with validation messages
        return View(team);
    }

    // GET: Teams/Edit/5
    public async Task<IActionResult> Edit(int? id)
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

    // POST: Teams/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Team team)
    {
        if (id != team.Id)
        {
            return NotFound();
        }

        // Remove properties that shouldn't be bound
        ModelState.Remove("OwnerId");
        ModelState.Remove("CreatedAt");
        ModelState.Remove("Owner");
        ModelState.Remove("Members");

        if (ModelState.IsValid)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var (success, errorMessage) = await _teamsService.UpdateTeamAsync(id, currentUser!.Id, team.Name, team.Description);

            if (!success)
            {
                ModelState.AddModelError("", errorMessage ?? "An error occurred");
                return View(team);
            }

            return RedirectToAction(nameof(Index));
        }

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
        var currentUser = await _userManager.GetUserAsync(User);
        var (success, errorMessage) = await _teamsService.InviteUserToTeamAsync(id, currentUser!.Id, email);

        if (!success)
        {
            var team = await _context.Teams
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (team == null)
            {
                return NotFound();
            }

            ModelState.AddModelError("", errorMessage ?? "An error occurred");
            return View(team);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Teams/Invitations
    public async Task<IActionResult> Invitations()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var pendingInvitations = await _teamsService.GetPendingInvitationsAsync(currentUser!.Id);
        return View(pendingInvitations);
    }

    // POST: Teams/AcceptInvite/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptInvite(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var result = await _teamsService.AcceptInvitationAsync(id, currentUser!.Id);

        if (!result)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Teams/DeclineInvite/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeclineInvite(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var result = await _teamsService.DeclineInvitationAsync(id, currentUser!.Id);

        if (!result)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Invitations));
    }
}