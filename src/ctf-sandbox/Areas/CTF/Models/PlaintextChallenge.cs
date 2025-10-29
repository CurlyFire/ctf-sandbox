using System.ComponentModel.DataAnnotations;

namespace ctf_sandbox.Areas.CTF.Models;

public class PlaintextChallenge : Challenge
{
    [Required]
    public string Content { get; set; } = string.Empty;
}
