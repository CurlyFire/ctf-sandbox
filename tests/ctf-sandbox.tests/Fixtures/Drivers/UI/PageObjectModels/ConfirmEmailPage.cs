using Microsoft.Playwright;

namespace ctf_sandbox.tests.Fixtures.Drivers.UI.PageObjectModels;

public class ConfirmEmailPage
{
    private readonly IPage _page;

    public ConfirmEmailPage(IPage page)
    {
        _page = page;
    }

    public async Task<bool> IsThankYouMessageVisible()
    {
        return await _page.GetByText("Thank you for confirming your email.").IsVisibleAsync();
    }

}
