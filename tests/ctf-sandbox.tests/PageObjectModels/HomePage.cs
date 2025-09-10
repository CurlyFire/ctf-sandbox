using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace ctf_sandbox.tests.PageObjectModels;

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

    public async Task VerifyMainLayoutComponents()
    {
        // Header with navigation
        await Expect(_page.GetByRole(AriaRole.Banner)).ToBeVisibleAsync();

        // Main navigation
        await Expect(_page.GetByRole(AriaRole.Navigation, new() { Name = "Main" })).ToBeVisibleAsync();

        // Dashboard link
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "View Dashboard" })).ToBeVisibleAsync();

        // Main content area
        await Expect(_page.GetByRole(AriaRole.Main)).ToBeVisibleAsync();

        // Footer
        await Expect(_page.GetByRole(AriaRole.Contentinfo)).ToBeVisibleAsync();

        // Brand logo/link
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "CTF Arena" })).ToBeVisibleAsync();
    }

    public async Task<string> GetLoggedInUsername()
    {
        var accountLink = _page.GetByRole(AriaRole.Link, new() { Name = "Manage Account Settings" });
        await Expect(accountLink).ToBeVisibleAsync();
        return (await accountLink.TextContentAsync()) ?? string.Empty;
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
