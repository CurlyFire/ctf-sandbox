using ctf_sandbox.tests.Core.Dsl.Scenario.Given;
using ctf_sandbox.tests.Core.Dsl.Scenario.Assume;
using ctf_sandbox.tests.Core.Dsl.Scenario.When;

namespace ctf_sandbox.tests.Core.Dsl.Scenario;

/// <summary>
/// Entry point: Scenario.Given()...When()...Then(). Nothing runs until the Then chain is awaited.
/// </summary>
public class ScenarioDsl
{
    private readonly UseCaseDsl _app;
    private bool _executed;

    public ScenarioDsl(UseCaseDsl app)
    {
        _app = app;
    }

    public AssumeStage Assume() => new(_app);

    public GivenStage Given()
    {
        EnsureNotExecuted();
        return new GivenStage(_app, this);
    }

    public WhenStage When()
    {
        EnsureNotExecuted();
        return new WhenStage(_app, this, new GivenStage(_app, this).Setup);
    }

    internal void MarkAsExecuted() => _executed = true;

    private void EnsureNotExecuted()
    {
        if (_executed)
        {
            throw new InvalidOperationException(
                "Scenario has already been executed. Each test method should contain only ONE scenario (Given-When-Then).");
        }
    }
}
