using ctf_sandbox.tests.Fixtures.Drivers;

namespace ctf_sandbox.tests.Fixtures.Dsl;

public class EmailsDsl
{
    private readonly IEmailsDriver _driver;

    public EmailsDsl(IEmailsDriver driver)
    {
        _driver = driver;
    }

    public async Task<bool> IsInboxAvailable()
    {
        return await _driver.IsInboxAvailable();
    }

    public async Task<bool> ConfirmRegistrationSentTo(string email)
    {
        return await _driver.ConfirmRegistrationSentTo(email);
    }
}