using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public class WhenSignIn : BaseWhen<VoidValue, VoidVerification>
{
    private string _userName = string.Empty;
    private string _password = string.Empty;

    internal WhenSignIn(UseCaseDsl app, ScenarioDsl scenario, Func<Task> setup)
        : base(app, scenario, setup)
    {
    }

    public WhenSignIn WithUserName(string userName)
    {
        _userName = userName;
        return this;
    }

    public WhenSignIn WithPassword(string password)
    {
        _password = password;
        return this;
    }

    protected override async Task<ExecutionResult<VoidValue, VoidVerification>> Execute(UseCaseDsl app)
    {
        var result = await app.CTF.SignIn().With(credentials =>
        {
            credentials.UserName = _userName;
            credentials.Password = _password;
        }).Execute();

        return new ExecutionResult<VoidValue, VoidVerification>(result);
    }
}
