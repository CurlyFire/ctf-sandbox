namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

public class ThenStage<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly Func<Task<ExecutionResult<TSuccessResponse, TSuccessVerification>>> _lazyExecute;
    private Task<ExecutionResult<TSuccessResponse, TSuccessVerification>>? _execution;

    internal ThenStage(UseCaseDsl app, Func<Task<ExecutionResult<TSuccessResponse, TSuccessVerification>>> lazyExecute)
    {
        App = app;
        _lazyExecute = lazyExecute;
    }

    internal UseCaseDsl App { get; }

    public ThenSuccess<TSuccessResponse, TSuccessVerification> ShouldSucceed() => new(this);

    public ThenFailure<TSuccessResponse, TSuccessVerification> ShouldFail() => new(this);

    // Runs the setup and the When operation once, on first use.
    internal Task<ExecutionResult<TSuccessResponse, TSuccessVerification>> GetExecutionResult()
        => _execution ??= _lazyExecute();
}
