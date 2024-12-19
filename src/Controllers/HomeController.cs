using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Models;
using ctf_sandbox.Data;

namespace ctf_sandbox.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var userId = _userManager.GetUserId(User);

        var userTeamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId && !tm.IsInvitePending)
            .Select(tm => tm.TeamId)
            .ToListAsync();

        var activeCompetitions = await _context.Competitions
            .Include(c => c.Teams)
                .ThenInclude(ct => ct.Team)
            .Where(c => c.StartDate <= now && c.EndDate >= now)
            .OrderBy(c => c.EndDate)
            .ToListAsync();

        // Separate competitions into those the user can participate in and others
        var viewModel = new HomeViewModel
        {
            ParticipatingCompetitions = activeCompetitions
                .Where(c => c.Teams.Any(ct => userTeamIds.Contains(ct.TeamId)))
                .ToList(),
            OtherCompetitions = activeCompetitions
                .Where(c => !c.Teams.Any(ct => userTeamIds.Contains(ct.TeamId)))
                .ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Competition(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var now = DateTime.UtcNow;
        var userId = _userManager.GetUserId(User);

        var userTeamIds = await _context.TeamMembers
            .Where(tm => tm.UserId == userId && !tm.IsInvitePending)
            .Select(tm => tm.TeamId)
            .ToListAsync();

        var competition = await _context.Competitions
            .Include(c => c.Teams)
                .ThenInclude(ct => ct.Team)
            .Include(c => c.Challenges)
                .ThenInclude(cc => cc.Challenge)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (competition == null)
        {
            return NotFound();
        }

        // Check if the competition is active and the user's team is participating
        if (now < competition.StartDate || now > competition.EndDate)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!competition.Teams.Any(ct => userTeamIds.Contains(ct.TeamId)))
        {
            return RedirectToAction(nameof(Index));
        }

        return View(competition);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
