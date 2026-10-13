namespace ctf_sandbox.tests.Core.Dsl.Scenario.Given.Steps;

public class GivenTeam : BaseGiven
{
    private string _teamName = ScenarioDefaults.DefaultTeamName;
    private uint _memberCount = ScenarioDefaults.DefaultMemberCount;

    internal GivenTeam(GivenStage givenStage) : base(givenStage)
    {
    }

    public GivenTeam WithTeamName(string teamName)
    {
        _teamName = teamName;
        return this;
    }

    public GivenTeam WithMemberCount(uint memberCount)
    {
        _memberCount = memberCount;
        return this;
    }

    internal override async Task Execute(UseCaseDsl app)
    {
        var result = await app.CTF.CreateTeam().With(team =>
        {
            team.TeamName = _teamName;
            team.MemberCount = _memberCount;
        }).Execute();

        result.ShouldSucceed();
    }
}
