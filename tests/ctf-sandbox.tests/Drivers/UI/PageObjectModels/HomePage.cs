using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.UI.PageObjectModels;

public class HomePage
{
    private readonly IPage _page;

    public HomePage(IPage page)
    {
        _page = page;
    }

    public async Task<string> GetPageTitle()
    {
        return await _page.TitleAsync();
    }

    public async Task<bool> IsBannerVisible()
    {
        return await _page.GetByRole(AriaRole.Banner).IsVisibleAsync();
    }

    public async Task<bool> IsMainNavigationVisible()
    {
        return await _page.GetByRole(AriaRole.Navigation, new() { Name = "Main" }).IsVisibleAsync();
    }

    public async Task<bool> IsDashboardLinkVisible()
    {
        return await _page.GetByRole(AriaRole.Link, new() { Name = "View Dashboard" }).IsVisibleAsync();
    }

    public async Task<bool> IsMainContentAreaVisible()
    {
        return await _page.GetByRole(AriaRole.Main).IsVisibleAsync();
    }

    public async Task<bool> IsFooterVisible()
    {
        return await _page.GetByRole(AriaRole.Contentinfo).IsVisibleAsync();
    }

    public async Task<bool> IsBrandLogoVisible()
    {
        return await _page.GetByRole(AriaRole.Link, new() { Name = "CTF Arena" }).IsVisibleAsync();
    }

    public async Task<bool> IsUserLoggedIn(string email)
    {
        var accountLink = _page.GetByRole(AriaRole.Link, new() { Name = "Manage Account Settings" });
        var textContent = await accountLink.TextContentAsync();
        return textContent != null && textContent.Contains(email);
    }

    public async Task<EmailsPage> GoToEmailsPage()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Open Email" }).ClickAsync();
        var mailpitPage = await _page.WaitForPopupAsync();
        await mailpitPage.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        return new EmailsPage(mailpitPage);
    }

    public async Task<CreateAccountPage> GoToCreateAccountPage()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Create account" }).ClickAsync();
        return new CreateAccountPage(_page);
    }

    public async Task<SignInPage> GoToSignInPage()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();
        return new SignInPage(_page);
    }

    public async Task<ManageTeamsPage> GoToManageTeamsPage()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Manage Teams" }).ClickAsync();
        return new ManageTeamsPage(_page);
    }

}
