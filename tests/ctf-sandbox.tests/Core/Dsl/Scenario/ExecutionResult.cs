namespace ctf_sandbox.tests.Core.Dsl.Scenario;

/// <summary>The outcome of the When step, kept so the Then stage can verify it.</summary>
public class ExecutionResult<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    public ExecutionResult(CTFUseCaseResult<TSuccessResponse, TSuccessVerification> result)
    {
        Result = result;
    }

    public CTFUseCaseResult<TSuccessResponse, TSuccessVerification> Result { get; }
}
