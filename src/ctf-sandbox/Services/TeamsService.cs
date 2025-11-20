using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ctf_sandbox.Services;

public class TeamsService : ITeamsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeamsService> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBannedWordsService _bannedWordsService;

    public TeamsService(
        ApplicationDbContext context,
        ILogger<TeamsService> logger,
        TimeProvider timeProvider,
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor,
        IBannedWordsService bannedWordsService)
    {
        _context = context;
        _logger = logger;
        _timeProvider = timeProvider;
        _userManager = userManager;
        _emailSender = emailSender;
        _httpContextAccessor = httpContextAccessor;
        _bannedWordsService = bannedWordsService;
    }

    public async Task<IEnumerable<Team>> GetTeamsForUserAsync(string userId)
    {
        var teams = await _context.Teams
            .Include(t => t.Owner)
            .Include(t => t.Members)
                .ThenInclude(m => m.User)
            .Where(t => t.OwnerId == userId || t.Members.Any(m => m.UserId == userId && !m.IsInvitePending))
            .ToListAsync();

        return teams;
    }

    public async Task<(bool Success, string? ErrorMessage, Team? Team)> CreateTeamAsync(string userId, string name, string? description, uint memberCount)
    {
        if (await _bannedWordsService.ContainsBannedWordAsync(name))
        {
            return (false, "Team name contains banned words", null);
        }

        var team = new Team
        {
            Name = name,
            Description = description,
            MemberCount = memberCount,
            OwnerId = userId,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return (true, null, team);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int teamId, string userId, string name, string? description, uint memberCount)
    {
        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            return (false, "Team not found");
        }

        if (team.OwnerId != userId)
        {
            return (false, "Only the team owner can update the team");
        }

        if (await _bannedWordsService.ContainsBannedWordAsync(name))
        {
            return (false, "Team name contains banned words");
        }

        team.Name = name;
        team.Description = description;
        team.MemberCount = memberCount;
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> InviteUserToTeamAsync(int teamId, string invitingUserId, string invitedUserEmail)
    {
        var team = await _context.Teams
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            return (false, "Team not found");
        }

        if (team.OwnerId != invitingUserId)
        {
            return (false, "Only the team owner can invite members");
        }

        var invitingUser = await _userManager.FindByIdAsync(invitingUserId);
        var invitedUser = await _userManager.FindByEmailAsync(invitedUserEmail);
        if (invitedUser == null)
        {
            return (false, "User not found");
        }

        var existingMember = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == invitedUser.Id);

        if (existingMember != null)
        {
            return (false, "User is already a member or has a pending invitation");
        }

        var teamMember = new TeamMember
        {
            TeamId = teamId,
            UserId = invitedUser.Id,
            JoinedAt = _timeProvider.GetUtcNow().UtcDateTime,
            IsInvitePending = true
        };

        _context.TeamMembers.Add(teamMember);
        await _context.SaveChangesAsync();

        // Construct invitations page URL from current request
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}";
        var invitationsPageUrl = $"{baseUrl}/CTF/Teams/Invitations";

        // Send invitation email
        var subject = $"Invitation to join team {team.Name}";
        var message = $@"
            <h2>Team Invitation</h2>
            <p>You have been invited to join the team {team.Name} by {invitingUser?.Email}.</p>
            <p>To accept or decline this invitation, please visit your <a href='{invitationsPageUrl}'>team invitations page</a>.</p>";
        
        await _emailSender.SendEmailAsync(invitedUserEmail, subject, message);

        return (true, null);
    }

    public async Task<IEnumerable<TeamMember>> GetPendingInvitationsAsync(string userId)
    {
        var pendingInvitations = await _context.TeamMembers
            .Include(tm => tm.Team)
            .Include(tm => tm.Team.Owner)
            .Where(tm => tm.UserId == userId && tm.IsInvitePending)
            .ToListAsync();

        return pendingInvitations;
    }

    public async Task<bool> AcceptInvitationAsync(int invitationId, string userId)
    {
        var invitation = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.Id == invitationId && tm.UserId == userId && tm.IsInvitePending);

        if (invitation == null)
        {
            return false;
        }

        invitation.IsInvitePending = false;
        invitation.JoinedAt = _timeProvider.GetUtcNow().UtcDateTime;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeclineInvitationAsync(int invitationId, string userId)
    {
        var invitation = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.Id == invitationId && tm.UserId == userId && tm.IsInvitePending);

        if (invitation == null)
        {
            return false;
        }

        _context.TeamMembers.Remove(invitation);
        await _context.SaveChangesAsync();

        return true;
    }
}
