using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ctf_sandbox.Models;

public class Competition
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string CreatorId { get; set; } = string.Empty;
    public IdentityUser Creator { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<CompetitionTeam> Teams { get; set; } = new List<CompetitionTeam>();
    public ICollection<CompetitionChallenge> Challenges { get; set; } = new List<CompetitionChallenge>();
}