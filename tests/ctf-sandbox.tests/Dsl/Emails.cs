using ctf_sandbox.tests.Drivers;

namespace ctf_sandbox.tests.Dsl;

public class Emails
{
    private readonly IEmailsDriver _driver;

    public Emails(IEmailsDriver driver)
    {
        _driver = driver;
    }

    public async Task ConfirmRegistrationSentTo(string email)
    {
        Assert.True(await _driver.IsRegistrationSentTo(email));
    }

    public async Task ConfirmInboxIsAvailable()
    {
        Assert.True(await _driver.IsInboxAvailable());
    }
}