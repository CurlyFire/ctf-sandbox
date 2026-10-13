using System.Runtime.CompilerServices;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

/// <summary>Awaitable on its own; use And() to verify the response or the system state.</summary>
public class ThenSuccess<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;

    internal ThenSuccess(ThenStage<TSuccessResponse, TSuccessVerification> stage)
    {
        _stage = stage;
    }

    public ThenSuccessAnd<TSuccessResponse, TSuccessVerification> And() => new(_stage);

    public TaskAwaiter GetAwaiter() => Execute().GetAwaiter();

    private async Task Execute()
    {
        var result = await _stage.GetExecutionResult();
        result.Result.ShouldSucceed();
    }
}
