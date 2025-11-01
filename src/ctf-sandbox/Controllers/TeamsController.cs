using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ctf_sandbox.Services;
using ctf_sandbox.Areas.CTF.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ctf_sandbox.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TeamsController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITeamsService _teamsService;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(
        UserManager<IdentityUser> userManager,
        ITeamsService teamsService,
        ILogger<TeamsController> logger)
    {
        _userManager = userManager;
        _teamsService = teamsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all teams for the authenticated user (owned or member of).
    /// </summary>
    /// <returns>List of teams with owner and member details</returns>
    /// <response code="200">Returns the list of teams</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTeams()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var teams = await _teamsService.GetTeamsForUserAsync(currentUser.Id);
        return Ok(teams);
    }
}
