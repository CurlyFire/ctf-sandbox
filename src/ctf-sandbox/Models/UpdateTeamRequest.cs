using System.ComponentModel.DataAnnotations;

namespace ctf_sandbox.Models;

public class UpdateTeamRequest
{
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
}
