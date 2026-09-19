using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class CreateAccount : CTFUseCase<VoidValue, VoidVerification>
{
    private readonly CreateAccountParameters _parameters;

    public CreateAccount(ICTFDriver driver, UseCaseContext context, CreateAccountParameters parameters)
        : base(driver, context)
    {
        _parameters = parameters;
    }

    public CreateAccount With(Action<CreateAccountParameters> configure)
    {
        configure(_parameters);
        return this;
    }

    public override async Task<CTFUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.CreateAccount(_parameters.Email, _parameters.Password);
        return new CTFUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}

public record CreateAccountParameters
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}