using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ctf_sandbox.Areas.CTF.Models;

public class Team
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(1, 8, ErrorMessage = "The {0} must be between {1} and {2}.")]
    public uint MemberCount { get; set; }
    
    public string OwnerId { get; set; } = string.Empty;
    public IdentityUser Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
}
