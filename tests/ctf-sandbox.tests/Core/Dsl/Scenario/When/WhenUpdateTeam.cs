using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public class WhenUpdateTeam : BaseWhen<GetTeamResponse, UpdateTeamVerification>
{
    private string _originalTeamName = string.Empty;
    private string _teamName = string.Empty;
    private string? _description;
    private uint _memberCount;

    internal WhenUpdateTeam(UseCaseDsl app, ScenarioDsl scenario, Func<Task> setup)
        : base(app, scenario, setup)
    {
    }

    public WhenUpdateTeam WithOriginalTeamName(string teamName)
    {
        _originalTeamName = teamName;
        return this;
    }

    public WhenUpdateTeam WithTeamName(string teamName)
    {
        _teamName = teamName;
        return this;
    }

    public WhenUpdateTeam WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public WhenUpdateTeam WithMemberCount(uint memberCount)
    {
        _memberCount = memberCount;
        return this;
    }

    protected override async Task<ExecutionResult<GetTeamResponse, UpdateTeamVerification>> Execute(UseCaseDsl app)
    {
        var result = await app.CTF.UpdateTeam().With(team =>
        {
            team.OriginalTeamName = _originalTeamName;
            team.TeamName = _teamName;
            team.Description = _description;
            team.MemberCount = _memberCount;
        }).Execute();

        return new ExecutionResult<GetTeamResponse, UpdateTeamVerification>(result);
    }
}
