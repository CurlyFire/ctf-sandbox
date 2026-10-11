namespace ctf_sandbox.tests.Core.Dsl.Scenario.Given.Steps;

public class GivenAccount : BaseGiven
{
    private string _email = ScenarioDefaults.DefaultEmail;
    private string _password = ScenarioDefaults.DefaultPassword;

    internal GivenAccount(GivenStage givenStage) : base(givenStage)
    {
    }

    public GivenAccount WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public GivenAccount WithPassword(string password)
    {
        _password = password;
        return this;
    }

    internal override async Task Execute(UseCaseDsl app)
    {
        (await app.CTF.CreateAccount().With(account =>
        {
            account.Email = _email;
            account.Password = _password;
        }).Execute()).ShouldSucceed();

        (await app.ExternalSystems.Emails.ActivateRegistrationSentTo(_email).Execute()).ShouldSucceed();
    }
}
