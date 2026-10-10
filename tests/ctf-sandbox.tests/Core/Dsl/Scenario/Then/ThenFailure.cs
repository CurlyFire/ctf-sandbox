using System.Runtime.CompilerServices;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

/// <summary>Awaitable: queued checks run when the chain is awaited.</summary>
public class ThenFailure<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;
    private readonly List<Action<SystemErrorFailureVerification>> _assertions = [];

    internal ThenFailure(ThenStage<TSuccessResponse, TSuccessVerification> stage)
    {
        _stage = stage;
    }

    public ThenFailure<TSuccessResponse, TSuccessVerification> ErrorMessage(string expectedMessage)
    {
        _assertions.Add(v => v.ErrorMessage(expectedMessage));
        return this;
    }

    public ThenFailure<TSuccessResponse, TSuccessVerification> FieldErrorMessage(string expectedField, string expectedMessage)
    {
        _assertions.Add(v => v.FieldErrorMessage(expectedField, expectedMessage));
        return this;
    }

    public TaskAwaiter GetAwaiter() => Execute().GetAwaiter();

    private async Task Execute()
    {
        var result = await _stage.GetExecutionResult();
        var verification = result.Result.ShouldFail();

        foreach (var assertion in _assertions)
        {
            assertion(verification);
        }
    }
}
