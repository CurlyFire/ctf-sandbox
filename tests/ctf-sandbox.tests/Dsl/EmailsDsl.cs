using System.Reflection;
using ctf_sandbox.tests.PageObjectModels;

namespace ctf_sandbox.tests.Dsl;

public class EmailsDsl
{
    private readonly EmailsPage _page;

    public EmailsDsl(EmailsPage page)
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