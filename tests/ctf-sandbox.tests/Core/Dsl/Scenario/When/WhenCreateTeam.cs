using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public class WhenCreateTeam : BaseWhen<GetTeamResponse, CreateTeamVerification>
{
    private string _teamName = ScenarioDefaults.DefaultTeamName;
    private uint _memberCount = ScenarioDefaults.DefaultMemberCount;
    private string? _rawMemberCount;

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

    /// <summary>For member counts that are not a valid number, e.g. "five".</summary>
    public WhenCreateTeam WithMemberCount(string memberCount)
    {
        _rawMemberCount = memberCount;
        return this;
    }

    protected override async Task<ExecutionResult<GetTeamResponse, CreateTeamVerification>> Execute(UseCaseDsl app)
    {
        var result = await app.CTF.CreateTeam().With(t =>
        {
            t.TeamName = _teamName;
            t.MemberCount = _memberCount;
            t.RawMemberCount = _rawMemberCount;
        }).Execute();

        return new ExecutionResult<GetTeamResponse, CreateTeamVerification>(result);
    }
}
