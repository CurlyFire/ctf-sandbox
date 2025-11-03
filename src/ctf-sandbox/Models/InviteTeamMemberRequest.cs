using System.ComponentModel.DataAnnotations;

namespace ctf_sandbox.Models;

public class InviteTeamMemberRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
