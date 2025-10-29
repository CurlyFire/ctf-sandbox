using System.ComponentModel.DataAnnotations;

namespace ctf_sandbox.Areas.CTF.Models;

public class CompetitionTeam
{
    public int Id { get; set; }

    public int CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public DateTime JoinedAt { get; set; }
    public int Score { get; set; }
}
