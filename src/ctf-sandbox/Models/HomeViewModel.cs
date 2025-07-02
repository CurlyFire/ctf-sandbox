namespace ctf_sandbox.Models;

public class HomeViewModel
{
    public IList<Competition> ParticipatingCompetitions { get; set; } = new List<Competition>();
    public IList<Competition> OtherCompetitions { get; set; } = new List<Competition>();
}