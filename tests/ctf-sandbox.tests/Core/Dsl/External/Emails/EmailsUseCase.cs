using ctf_sandbox.tests.Core.Dsl;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Emails;

public abstract class EmailsUseCase<TResponse, TVerification>
    where TVerification : ResponseVerification<TResponse>
{
    protected IEmailsDriver Driver { get; }
    protected UseCaseContext Context { get; }

    protected EmailsUseCase(IEmailsDriver driver, UseCaseContext context)
    {
        Driver = driver;
        Context = context;
    }

    public abstract Task<EmailsUseCaseResult<TResponse, TVerification>> Execute();
}