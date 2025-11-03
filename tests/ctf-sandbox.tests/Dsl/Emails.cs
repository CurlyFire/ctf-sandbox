using ctf_sandbox.tests.Drivers;

namespace ctf_sandbox.tests.Dsl;

public class Emails
{
    private readonly IEmailsDriver _driver;

    public Emails(IEmailsDriver driver)
    {
        _driver = driver;
    }

    public async Task ActivateRegistrationSentTo(string email)
    {
        await _driver.ActivateRegistrationSentTo(email);
    }
}