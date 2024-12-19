namespace ctf_sandbox.Models;

public class CompetitionChallenge
{
    public int Id { get; set; }

    public int CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;

    public int ChallengeId { get; set; }
    public Challenge Challenge { get; set; } = null!;

    public DateTime AddedAt { get; set; }
    public int Points { get; set; }
}