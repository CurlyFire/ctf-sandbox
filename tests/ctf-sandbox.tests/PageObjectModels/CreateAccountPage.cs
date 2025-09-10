using Microsoft.Playwright;

namespace ctf_sandbox.tests.PageObjectModels;

public class CreateAccountPage
{
    private readonly IPage _page;

    public CreateAccountPage(IPage page)
    {
        _page = page;
    }

    public async Task FillEmail(string email)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
    }
    public async Task FillPassword(string password)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
    }
    public async Task FillConfirmPassword(string password)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password", Exact = true }).FillAsync(password);
    }
    public async Task<ConfirmAccountCreationPage> CreateAccount()
    {
        await _page.GetByRole(AriaRole.Button, new() { Name = "CREATE ACCOUNT" }).ClickAsync();
        return new ConfirmAccountCreationPage(_page);
    }
}
