using ctf_sandbox.tests.Core.Dsl.Scenario.Given.Steps;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Given;

/// <summary>
/// Collects the preconditions. They are applied in Setup(), which runs lazily when the Then chain is awaited.
/// </summary>
public class GivenStage
{
    private readonly UseCaseDsl _app;
    private readonly ScenarioDsl _scenario;
    private readonly List<Func<Task>> _setupActions = [];
    private readonly List<BaseGiven> _steps = [];
    private bool _signedIn = true;

    internal GivenStage(UseCaseDsl app, ScenarioDsl scenario)
    {
        _app = app;
        _scenario = scenario;
    }

    public GivenStage SignedIn()
    {
        _signedIn = true;
        return this;
    }

    public GivenStage NotSignedIn()
    {
        _signedIn = false;
        return this;
    }

    public GivenStage TeamCreated(string teamName, uint memberCount)
    {
        _setupActions.Add(async () =>
        {
            var result = await _app.CTF.CreateTeam().With(team =>
            {
                team.TeamName = teamName;
                team.MemberCount = memberCount;
            }).Execute();

            result.ShouldSucceed();
        });

        return this;
    }

    public GivenBannedWord BannedWord()
    {
        var step = new GivenBannedWord(this);
        _steps.Add(step);
        return step;
    }

    public GivenAccount Account()
    {
        var step = new GivenAccount(this);
        _steps.Add(step);
        return step;
    }

    public GivenStage And() => this;

    public When.WhenStage When() => new(_app, _scenario, Setup);

    internal async Task Setup()
    {
        if (_signedIn)
        {
            (await _app.CTF.SignIn().Execute()).ShouldSucceed();
        }

        foreach (var setupAction in _setupActions)
        {
            await setupAction();
        }

        foreach (var step in _steps)
        {
            await step.Execute(_app);
        }
    }
}
