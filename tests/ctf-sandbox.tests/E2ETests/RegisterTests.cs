using Microsoft.Playwright;

namespace ctf_sandbox.tests.E2ETests;

public class RegisterTests : WebServerPageTest
{
    public RegisterTests(WebServer webServer) : base(webServer)
    {
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ShouldBeAbleToRegister()
    {
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        await Page.GotoAsync(string.Empty);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create account" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(randomEmail);
        var password = "RegisterTest123!";
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password", Exact = true }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "CREATE ACCOUNT" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Open Email" }).ClickAsync();
        var mailpit = await Page.WaitForPopupAsync();
        await mailpit.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await mailpit.GetByRole(AriaRole.Button, new() { Name = "Inbox" }).ClickAsync();
        await mailpit.GetByRole(AriaRole.Link, new() { Name = $"CTF Arena To: {randomEmail}" }).ClickAsync();
        await mailpit.Locator("#preview-html").ContentFrame.GetByRole(AriaRole.Link, new() { Name = "clicking here" }).ClickAsync();
        var emailDetail = await mailpit.WaitForPopupAsync();
        await emailDetail.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Expect(emailDetail.GetByText("Thank you for confirming your email.")).ToBeVisibleAsync();
    }
}
