using Microsoft.Playwright;

namespace ctf_sandbox.tests.Fixture.Drivers.UI.PageObjectModels;

public class ConfirmAccountCreationPage
{
    private readonly IPage _page;

    public ConfirmAccountCreationPage(IPage page)
    {
        _page = page;
    }

    public async Task<bool> IsConfirmationMessageVisible()
    {
        return await _page.GetByText("Please check your email to confirm your account.").IsVisibleAsync();
    }

}
