using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Emails;

public class EmailsDsl
{
    private readonly IEmailsDriver _driver;

    public EmailsDsl(IEmailsDriver driver)
    {
        _driver = driver;
    }

    public async Task ActivateRegistrationSentTo(string email)
    {
        await _driver.ActivateRegistrationSentTo(email);
    }
}