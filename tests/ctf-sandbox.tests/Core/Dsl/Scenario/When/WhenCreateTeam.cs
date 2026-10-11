using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public class WhenCreateTeam : BaseWhen<Team, CreateTeamVerification>
{
    private string _teamName = ScenarioDefaults.DefaultTeamName;
    private uint _memberCount = ScenarioDefaults.DefaultMemberCount;

    internal WhenCreateTeam(UseCaseDsl app, ScenarioDsl scenario, Func<Task> setup)
        : base(app, scenario, setup)
    {
    }

    public WhenCreateTeam WithTeamName(string teamName)
    {
        _teamName = teamName;
        return this;
    }

    public WhenCreateTeam WithMemberCount(uint memberCount)
    {
        _memberCount = memberCount;
        return this;
    }

    protected override async Task<ExecutionResult<Team, CreateTeamVerification>> Execute(UseCaseDsl app)
    {
        var result = await app.CTF.CreateTeam().With(t =>
        {
            t.TeamName = _teamName;
            t.MemberCount = _memberCount;
        }).Execute();

        return new ExecutionResult<Team, CreateTeamVerification>(result);
    }
}
