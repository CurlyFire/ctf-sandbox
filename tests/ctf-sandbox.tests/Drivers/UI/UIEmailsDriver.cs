using ctf_sandbox.tests.Drivers.UI.PageObjectModels;

namespace ctf_sandbox.tests.Drivers.UI;

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

    public async Task ActivateRegistrationSentTo(string email)
    {
        var confirmEmailPage = await _page.OpenLatestEmailSentToAndOpenConfirmationLink(email);
        if (!await confirmEmailPage.IsThankYouMessageVisible())
        {
            throw new InvalidOperationException("The thank you message was not visible after confirming the email.");
        }
    }
}
