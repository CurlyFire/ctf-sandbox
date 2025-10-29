using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ctf_sandbox.Areas.CTF.Models;

public abstract class Challenge
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Flag { get; set; } = string.Empty;

    public int Points { get; set; }

    public string Category { get; set; } = string.Empty;

    public string CreatorId { get; set; } = string.Empty;
    public IdentityUser Creator { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string ChallengeType { get; set; } = string.Empty;
}
