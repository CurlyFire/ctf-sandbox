using ctf_sandbox.Areas.CTF.Models;

namespace ctf_sandbox.Services;

public interface ITeamsService
{
    Task<IEnumerable<Team>> GetTeamsForUserAsync(string userId);
    Task<(bool Success, string? ErrorMessage, Team? Team)> CreateTeamAsync(string userId, string name, string? description, uint memberCount);
    Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int teamId, string userId, string name, string? description, uint memberCount);
    Task<(bool Success, string? ErrorMessage)> InviteUserToTeamAsync(int teamId, string invitingUserId, string invitedUserEmail);
    Task<IEnumerable<TeamMember>> GetPendingInvitationsAsync(string userId);
    Task<bool> AcceptInvitationAsync(int invitationId, string userId);
    Task<bool> DeclineInvitationAsync(int invitationId, string userId);
}
