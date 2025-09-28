using ctf_sandbox.tests.Fixtures.Drivers.UI.PageObjectModels;

namespace ctf_sandbox.tests.Fixtures.Drivers.UI;

public class UIEmailsDriver : IEmailsDriver
{
    private readonly EmailsPage _page;

    public UIEmailsDriver(EmailsPage page)
    {
        _page = page;
    }

    public async Task<bool> IsInboxAvailable()
    {
        return await _page.IsInboxVisible();
    }

    public async Task<bool> ConfirmRegistrationSentTo(string email)
    {
        var confirmEmailPage = await _page.OpenLatestEmailSentToAndOpenConfirmationLink(email);
        return await confirmEmailPage.IsThankYouMessageVisible();
    }

}
