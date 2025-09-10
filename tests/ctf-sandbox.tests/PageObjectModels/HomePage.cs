using Microsoft.Playwright;

namespace ctf_sandbox.tests.PageObjectModels;

public class HomePage
{
    private readonly IPage _page;

    public HomePage(IPage page)
    {
        _page = page;
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
