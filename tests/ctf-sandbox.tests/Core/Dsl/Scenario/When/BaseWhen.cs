using ctf_sandbox.tests.Core.Dsl.Scenario.Then;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public abstract class BaseWhen<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly UseCaseDsl _app;
    private readonly ScenarioDsl _scenario;
    private readonly Func<Task> _setup;

    protected BaseWhen(UseCaseDsl app, ScenarioDsl scenario, Func<Task> setup)
    {
        _app = app;
        _scenario = scenario;
        _setup = setup;
    }

    public ThenStage<TSuccessResponse, TSuccessVerification> Then()
    {
        _scenario.MarkAsExecuted();
        return new ThenStage<TSuccessResponse, TSuccessVerification>(_app, async () =>
        {
            await _setup();
            return await Execute(_app);
        });
    }

    protected abstract Task<ExecutionResult<TSuccessResponse, TSuccessVerification>> Execute(UseCaseDsl app);
}
