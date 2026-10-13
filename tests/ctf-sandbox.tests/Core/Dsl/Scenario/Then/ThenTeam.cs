using ctf_sandbox.tests.Core.Dsl.UseCases;

using System.Runtime.CompilerServices;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

public class ThenTeam<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;
    private readonly string _teamName;
    private readonly List<Action<GetTeamVerification>> _verifications = [];

    internal ThenTeam(ThenStage<TSuccessResponse, TSuccessVerification> stage, string teamName)
    {
        _stage = stage;
        _teamName = teamName;
    }

    public ThenTeam<TSuccessResponse, TSuccessVerification> HasTeamName(string expectedTeamName)
    {
        _verifications.Add(v => v.HasTeamName(expectedTeamName));
        return this;
    }

    public ThenTeam<TSuccessResponse, TSuccessVerification> HasMemberCount(uint expectedMemberCount)
    {
        _verifications.Add(v => v.HasMemberCount(expectedMemberCount));
        return this;
    }

    public ThenTeam<TSuccessResponse, TSuccessVerification> HasDescription(string expectedDescription)
    {
        _verifications.Add(v => v.HasDescription(expectedDescription));
        return this;
    }

    public TaskAwaiter GetAwaiter() => Execute().GetAwaiter();

    private async Task Execute()
    {
        var result = await _stage.GetExecutionResult();
        result.Result.ShouldSucceed();

        var team = await _stage.App.CTF.GetTeam().WithTeamName(_teamName).Execute();
        var verification = team.ShouldSucceed();

        foreach (var check in _verifications)
        {
            check(verification);
        }
    }
}
