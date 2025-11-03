using Microsoft.AspNetCore.Identity;

namespace ctf_sandbox.Areas.CTF.Models;

public class TeamMember
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public IdentityUser User { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
    public bool IsInvitePending { get; set; }
}
