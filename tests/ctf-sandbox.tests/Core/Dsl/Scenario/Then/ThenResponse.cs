using System.Runtime.CompilerServices;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

public class ThenResponse<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;
    private readonly Action<TSuccessVerification> _verification;

    internal ThenResponse(ThenStage<TSuccessResponse, TSuccessVerification> stage, Action<TSuccessVerification> verification)
    {
        _stage = stage;
        _verification = verification;
    }

    public TaskAwaiter GetAwaiter() => Execute().GetAwaiter();

    private async Task Execute()
    {
        var result = await _stage.GetExecutionResult();
        _verification(result.Result.ShouldSucceed());
    }
}
