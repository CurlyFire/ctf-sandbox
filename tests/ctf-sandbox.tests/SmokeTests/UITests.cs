using ctf_sandbox.tests.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.SmokeTests;

public class UITests : WebServerPageTest
{
    public UITests(WebServer webServer) : base(webServer)
    {
    }

    [Trait("Category", "Smoke_UI")]
    [Fact]
    public async Task ShouldLoadMainPage()
    {
        // Go to home page
        var response = await Page.GotoAsync(string.Empty);

        // Check if the response is successful
        Assert.True(response?.Ok, "Failed to load the home page");
        // Check if the page title is correct
        var title = await Page.TitleAsync();
        Assert.Equal("Home Page - CTF Arena", title);

        // Verify main layout components

        // Header with navigation
        await Expect(Page.GetByRole(AriaRole.Banner)).ToBeVisibleAsync();

        // Main navigation
        await Expect(Page.GetByRole(AriaRole.Navigation, new() { Name = "Main" })).ToBeVisibleAsync();

        // Dashboard link should always be present
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "View Dashboard" })).ToBeVisibleAsync();

        // Main content area
        await Expect(Page.GetByRole(AriaRole.Main)).ToBeVisibleAsync();

        // Footer
        await Expect(Page.GetByRole(AriaRole.Contentinfo)).ToBeVisibleAsync();

        // Brand logo/link
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "CTF Arena" })).ToBeVisibleAsync();
    }

    [Trait("Category", "Smoke_UI")]
    [Fact]
    public async Task ShouldLoginWithValidCredentials()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.dev.json", optional: true)
            .AddEnvironmentVariables();
        var config = configBuilder.Build();
        var username = config.GetRequiredValue<string>("AdminAccount:Email");
        var password = config.GetRequiredValue<string>("AdminAccount:Password");
        // Navigate to login page
        await Page.GotoAsync("/Identity/Account/Login");
        // Fill in the login form
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Handle" }).FillAsync(username);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Access Code" }).FillAsync(password);
        // Submit the form
        await Page.GetByRole(AriaRole.Button, new() { Name = "AUTHENTICATE" }).ClickAsync();
        // Wait for navigation to the main page
        await Page.WaitForURLAsync(string.Empty);

        // Verify that the user's email is displayed in the navigation
        var accountLink = Page.GetByRole(AriaRole.Link, new() { Name = "Manage Account Settings" });
        await Expect(accountLink).ToBeVisibleAsync();
        await Expect(accountLink.GetByText(username)).ToBeVisibleAsync();
    }
}