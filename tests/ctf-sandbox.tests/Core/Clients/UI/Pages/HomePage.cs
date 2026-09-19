using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class HomePage : ErrorPage
{
    public HomePage(IPage page) : base(page)
    {
    }

    public async Task<string> GetPageTitle()
    {
        return await Page.TitleAsync();
    }

    public async Task<bool> IsBannerVisible()
    {
        return await Page.GetByRole(AriaRole.Banner).IsVisibleAsync();
    }

    public async Task<bool> IsMainNavigationVisible()
    {
        return await Page.GetByRole(AriaRole.Navigation, new() { Name = "Main" }).IsVisibleAsync();
    }

    public async Task<bool> IsDashboardLinkVisible()
    {
        return await Page.GetByRole(AriaRole.Link, new() { Name = "View Dashboard" }).IsVisibleAsync();
    }

    public async Task<bool> IsMainContentAreaVisible()
    {
        return await Page.GetByRole(AriaRole.Main).IsVisibleAsync();
    }

    public async Task<bool> IsFooterVisible()
    {
        return await Page.GetByRole(AriaRole.Contentinfo).IsVisibleAsync();
    }

    public async Task<bool> IsBrandLogoVisible()
    {
        return await Page.GetByRole(AriaRole.Link, new() { Name = "CTF Arena" }).IsVisibleAsync();
    }

    public async Task<bool> IsUserLoggedIn(string email)
    {
        var accountLink = Page.GetByRole(AriaRole.Link, new() { Name = "Manage Account Settings" });
        var textContent = await accountLink.TextContentAsync();
        return textContent != null && textContent.Contains(email);
    }

    public async Task<CreateAccountPage> GoToCreateAccountPage()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create account" }).ClickAsync();
        return new CreateAccountPage(Page);
    }

    public async Task<SignInPage> GoToSignInPage()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();
        return new SignInPage(Page);
    }

    public async Task<ManageTeamsPage> GoToManageTeamsPage()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage Teams" }).ClickAsync();
        return new ManageTeamsPage(Page);
    }

    internal async Task<IpInfoPage> GoToIpInfoPage()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "IP Address Lookup" }).ClickAsync();
        return new IpInfoPage(Page);
    }
}
