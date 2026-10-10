namespace ctf_sandbox.tests.Core.Dsl.Scenario.Given;

/// <summary>
/// Collects the preconditions. They are applied in Setup(), which runs lazily when the Then chain is awaited.
/// </summary>
public class GivenStage
{
    private readonly UseCaseDsl _app;
    private readonly ScenarioDsl _scenario;
    private readonly List<Func<Task>> _setupActions = [];
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

    public GivenStage BannedWordExists(string word)
    {
        _setupActions.Add(() => _app.ExternalSystems.BannedWords.CreateBannedWord(word));
        return this;
    }

    public GivenStage AccountRegistered(string email, string password)
    {
        _setupActions.Add(async () =>
        {
            (await _app.CTF.CreateAccount().With(account =>
            {
                account.Email = email;
                account.Password = password;
            }).Execute()).ShouldSucceed();

            (await _app.ExternalSystems.Emails.ActivateRegistrationSentTo(email).Execute()).ShouldSucceed();
        });

        return this;
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
    }
}
