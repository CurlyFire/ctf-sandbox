using ctf_sandbox.tests.Core.Dsl.UseCases;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using ctf_sandbox.tests.Core.Dsl.External.Emails.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.External.Emails;

public class EmailsDsl
{
    private readonly IEmailsDriver _driver;
    private readonly UseCaseFactory<IEmailsDriver> _useCaseFactory;

    public EmailsDsl(IEmailsDriver driver, IServiceProvider serviceProvider)
    {
        _driver = driver;
        _useCaseFactory = new UseCaseFactory<IEmailsDriver>(driver, serviceProvider);
    }

    public GoToMailpit GoToMailpit() => _useCaseFactory.Create<GoToMailpit>();

    public async Task ActivateRegistrationSentTo(string email)
    {
        await _driver.ActivateRegistrationSentTo(email);
    }
}