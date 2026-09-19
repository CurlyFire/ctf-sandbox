using ctf_sandbox.tests.Core.Drivers.CTF;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class SignIn : CTFUseCase<VoidValue, VoidVerification>
{
    private readonly SignInParameters _parameters;

    public SignIn(ICTFDriver driver, UseCaseContext context, SignInParameters parameters) : base(driver, context)
    {
        _parameters = parameters;
    }

    public SignIn With(Action<SignInParameters> configure)
    {
        configure(_parameters);
        return this;
    }


    public override async Task<CTFUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.SignIn(_parameters.UserName, _parameters.Password);
        return new CTFUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}

public record SignInParameters
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public SignInParameters(CTFConfiguration config)
    {
        UserName = config.WebServerCredentials.Username;
        Password = config.WebServerCredentials.Password;
    }
}
