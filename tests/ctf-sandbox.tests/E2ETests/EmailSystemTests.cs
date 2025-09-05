using Microsoft.Playwright;

namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : WebServerPageTest
{
    public EmailSystemTests(WebServer webServer) : base(webServer)
    {
    }

    [Trait("Category", "E2E")]
    [Fact]
    public async Task ShouldBeAbleToViewEmails()
    {
        await Page.GotoAsync(string.Empty);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Open Email" }).ClickAsync();
        var mailpit = await Page.WaitForPopupAsync();
        await mailpit.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Expect(mailpit.GetByRole(AriaRole.Button, new() { Name = "Inbox" })).ToBeVisibleAsync();
    }
}

