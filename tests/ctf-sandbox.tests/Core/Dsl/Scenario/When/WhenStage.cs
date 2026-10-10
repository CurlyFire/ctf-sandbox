namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public class WhenStage
{
    private readonly UseCaseDsl _app;
    private readonly ScenarioDsl _scenario;
    private readonly Func<Task> _setup;

    internal WhenStage(UseCaseDsl app, ScenarioDsl scenario, Func<Task> setup)
    {
        _app = app;
        _scenario = scenario;
        _setup = setup;
    }

    public WhenCreateTeam CreateTeam() => new(_app, _scenario, _setup);

    public WhenUpdateTeam UpdateTeam() => new(_app, _scenario, _setup);

    public WhenGetIpInfo GetIpInfo() => new(_app, _scenario, _setup);

    public WhenSignIn SignIn() => new(_app, _scenario, _setup);
}
