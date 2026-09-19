using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class ConfirmEmailPage : ErrorPage
{
    public ConfirmEmailPage(IPage page) : base(page)
    {
    }

    public async Task<bool> IsThankYouMessageVisible()
    {
        return await Page.GetByText("Thank you for confirming your email.").IsVisibleAsync();
    }

}
