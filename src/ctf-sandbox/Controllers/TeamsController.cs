using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ctf_sandbox.Services;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
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

    /// <summary>
    /// Creates a new team with the authenticated user as the owner.
    /// </summary>
    /// <param name="request">Team creation request containing name and optional description</param>
    /// <returns>The newly created team</returns>
    /// <response code="201">Returns the newly created team</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var team = await _teamsService.CreateTeamAsync(currentUser.Id, request.Name, request.Description);
        return CreatedAtAction(nameof(GetTeams), new { id = team.Id }, team);
    }

    /// <summary>
    /// Updates an existing team. Only the team owner can update the team.
    /// </summary>
    /// <param name="teamId">The ID of the team to update</param>
    /// <param name="request">Team update request containing name and optional description</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the team was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not the team owner</response>
    /// <response code="404">If the team was not found</response>
    [HttpPut("{teamId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] UpdateTeamRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var (success, errorMessage) = await _teamsService.UpdateTeamAsync(teamId, currentUser.Id, request.Name, request.Description);

        if (!success)
        {
            if (errorMessage == "Team not found")
            {
                return NotFound(new { message = errorMessage });
            }
            if (errorMessage == "Only the team owner can update the team")
            {
                return Forbid();
            }
            return BadRequest(new { message = errorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Invites a user to join a team by email. Only the team owner can invite members.
    /// </summary>
    /// <param name="teamId">The ID of the team</param>
    /// <param name="request">Invitation request containing the user's email</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the invitation was sent successfully</response>
    /// <response code="400">If the request is invalid or the invitation failed</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the team was not found</response>
    [HttpPost("{teamId}/invite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InviteTeamMember(int teamId, [FromBody] InviteTeamMemberRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var (success, errorMessage) = await _teamsService.InviteUserToTeamAsync(teamId, currentUser.Id, request.Email);

        if (!success)
        {
            if (errorMessage == "Team not found")
            {
                return NotFound(new { message = errorMessage });
            }
            return BadRequest(new { message = errorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Gets all pending team invitations for the authenticated user.
    /// </summary>
    /// <returns>List of pending invitations</returns>
    /// <response code="200">Returns the list of pending invitations</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpGet("invitations")]
    [ProducesResponseType(typeof(IEnumerable<TeamMember>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPendingInvitations()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var invitations = await _teamsService.GetPendingInvitationsAsync(currentUser.Id);
        return Ok(invitations);
    }

    /// <summary>
    /// Accepts a team invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the invitation was accepted successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the invitation was not found</response>
    [HttpPost("invitations/{invitationId}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptInvitation(int invitationId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = await _teamsService.AcceptInvitationAsync(invitationId, currentUser.Id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Declines a team invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation</param>
    /// <returns>Success status</returns>
    /// <response code="204">If the invitation was declined successfully</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the invitation was not found</response>
    [HttpPost("invitations/{invitationId}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeclineInvitation(int invitationId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Unauthorized();
        }

        var result = await _teamsService.DeclineInvitationAsync(invitationId, currentUser.Id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
