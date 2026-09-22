using ctf_sandbox.tests.Core;
using ctf_sandbox.tests.Core.Dsl;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Emails.UseCases;

public class GoToMailpit : EmailsUseCase<VoidValue, VoidVerification>
{
    public GoToMailpit(IEmailsDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public override async Task<EmailsUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.GoToMailpit();
        return new EmailsUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}