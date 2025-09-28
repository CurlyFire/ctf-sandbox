using Microsoft.Playwright;

namespace ctf_sandbox.tests.Fixtures.Drivers.UI.PageObjectModels;

public class EmailsPage
{
    private readonly IPage _page;

    public EmailsPage(IPage page)
    {
        _page = page;
    }

    public async Task<bool> IsInboxVisible()
    {
        return await _page.GetByRole(AriaRole.Button, new() { Name = "Inbox" }).IsVisibleAsync();
    }

    public async Task<ConfirmEmailPage> OpenLatestEmailSentToAndOpenConfirmationLink(string email)
    {
        await _page.GetByRole(AriaRole.Button, new() { Name = "Inbox" }).ClickAsync();
        await _page.GetByRole(AriaRole.Link, new() { Name = $"CTF Arena To: {email}" }).ClickAsync();
        await _page.Locator("#preview-html").ContentFrame.GetByRole(AriaRole.Link, new() { Name = "clicking here" }).ClickAsync();
        var confirmEmailPage = await _page.WaitForPopupAsync();
        await confirmEmailPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        return new ConfirmEmailPage(confirmEmailPage);
    }
}
