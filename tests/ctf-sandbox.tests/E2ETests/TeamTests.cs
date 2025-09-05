using Microsoft.Playwright;

namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : WebServerPageTest
{
    public TeamTests(WebServer webServer) : base(webServer)
    {
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ShouldBeAbleToCreateTeam()
    {
        await Page.GotoAsync(string.Empty);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Handle" }).FillAsync(WebServer.WebServerCredentials.Username);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Access Code" }).FillAsync(WebServer.WebServerCredentials.Password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "AUTHENTICATE" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage Teams" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create New Team" }).ClickAsync();
        var randomTeamName = $"team_{Guid.NewGuid()}";
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync(randomTeamName);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
        await Expect(Page.GetByText($"{randomTeamName} Owner: {WebServer.WebServerCredentials.Username}")).ToBeVisibleAsync();
    }
}
