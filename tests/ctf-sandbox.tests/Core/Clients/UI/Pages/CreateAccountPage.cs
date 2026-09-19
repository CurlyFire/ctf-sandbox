using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class CreateAccountPage : ErrorPage
{
    public CreateAccountPage(IPage page) : base(page)
    {
    }

    public async Task FillEmail(string email)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
    }
    public async Task FillPassword(string password)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
    }
    public async Task FillConfirmPassword(string password)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password", Exact = true }).FillAsync(password);
    }
    public async Task<ConfirmAccountCreationPage> CreateAccount()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "CREATE ACCOUNT" }).ClickAsync();
        return new ConfirmAccountCreationPage(Page);
    }
}
