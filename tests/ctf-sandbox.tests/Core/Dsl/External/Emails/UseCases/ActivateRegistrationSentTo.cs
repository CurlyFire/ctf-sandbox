using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Emails.UseCases;

public class ActivateRegistrationSentTo : EmailsUseCase<VoidValue, VoidVerification>
{
    private string? _email;

    public ActivateRegistrationSentTo(IEmailsDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public ActivateRegistrationSentTo With(string email)
    {
        _email = email;
        return this;
    }

    public override async Task<EmailsUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        if (string.IsNullOrWhiteSpace(_email))
        {
            throw new InvalidOperationException("Email is required");
        }

        var result = await Driver.ActivateRegistrationSentTo(_email);
        return new EmailsUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}