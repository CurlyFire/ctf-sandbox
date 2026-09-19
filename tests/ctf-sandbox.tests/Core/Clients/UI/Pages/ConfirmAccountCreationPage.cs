using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class ConfirmAccountCreationPage : ErrorPage
{
    public ConfirmAccountCreationPage(IPage page) : base(page)
    {
    }

    public async Task<bool> IsConfirmationMessageVisible()
    {
        return await Page.GetByText("Please check your email to confirm your account.").IsVisibleAsync();
    }

}
