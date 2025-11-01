using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Data;
using ctf_sandbox.Models;

namespace ctf_sandbox.Areas.CTF.Controllers;

[Area("CTF")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TimeProvider _timeProvider;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        TimeProvider timeProvider)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
