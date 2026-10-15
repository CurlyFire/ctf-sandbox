namespace ctf_sandbox.tests.Core.Dsl.Scenario.Assume;

public class AssumeStage
{
    private readonly UseCaseDsl _app;

    public AssumeStage(UseCaseDsl app)
    {
        _app = app;
    }

    public AssumeRunningAction MyShop()
    {
        return new AssumeRunningAction(async () =>
        {
            (await _app.CTF.GoToCTF().Execute()).ShouldSucceed();
        }, this);
    }

    public AssumeRunningAction Mailpit()
    {
        return new AssumeRunningAction(async () =>
        {
            (await _app.ExternalSystems.Emails.GoToMailpit().Execute()).ShouldSucceed();
        }, this);
    }

    public AssumeRunningAction IpInfo()
    {
        return new AssumeRunningAction(async () =>
        {
            (await _app.ExternalSystems.IpInfo.GoToIpInfo().Execute()).ShouldSucceed();
        }, this);
    }

    public AssumeRunningAction Clock()
    {
        return new AssumeRunningAction(async () =>
        {
            (await _app.ExternalSystems.Clock.GoToClock().Execute()).ShouldSucceed();
        }, this);
    }

    public AssumeRunningAction BannedWords()
    {
        return new AssumeRunningAction(async () =>
        {
            (await _app.ExternalSystems.BannedWords.GoToBannedWords().Execute()).ShouldSucceed();
        }, this);
    }
}

public class AssumeRunningAction
{
    private readonly Func<Task> _action;
    private readonly AssumeStage _assumeStage;

    internal AssumeRunningAction(Func<Task> action, AssumeStage assumeStage)
    {
        _action = action;
        _assumeStage = assumeStage;
    }

    public async Task<AssumeStage> ShouldBeRunning()
    {
        await _action();
        return _assumeStage;
    }
}
