using System.Runtime.CompilerServices;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

/// <summary>Awaitable: queued checks run when the chain is awaited.</summary>
public class ThenSuccess<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;
    private readonly List<Action<TSuccessVerification>> _verifications = [];

    internal ThenSuccess(ThenStage<TSuccessResponse, TSuccessVerification> stage)
    {
        _stage = stage;
    }

    public ThenSuccess<TSuccessResponse, TSuccessVerification> Satisfies(Action<TSuccessVerification> verification)
    {
        _verifications.Add(verification);
        return this;
    }

    public TaskAwaiter GetAwaiter() => Execute().GetAwaiter();

    private async Task Execute()
    {
        var result = await _stage.GetExecutionResult();
        var verification = result.Result.ShouldSucceed();

        foreach (var check in _verifications)
        {
            check(verification);
        }
    }
}
