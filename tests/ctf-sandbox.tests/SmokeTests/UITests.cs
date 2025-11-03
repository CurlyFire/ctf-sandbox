using ctf_sandbox.tests.Drivers.UI.PageObjectModels;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class UITests : PageObjectModelTests
{
    private HomePage _homePage;

    public UITests(RealExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
        _homePage = GetHomePage();
    }

    [Trait("Category", "Smoke_UI")]
    [Fact]
    public async Task ShouldLoadMainPage()
    {

        // Check if the page title is correct
        var title = await _homePage.GetPageTitle();
        Assert.Equal("Home Page - CTF Arena", title);

        // Verify each main layout component individually
        Assert.True(await _homePage.IsBannerVisible(), "Header banner should be visible on the home page");
        Assert.True(await _homePage.IsMainNavigationVisible(), "Main navigation menu should be visible on the home page");
        Assert.True(await _homePage.IsDashboardLinkVisible(), "Dashboard link should be visible on the home page");
        Assert.True(await _homePage.IsMainContentAreaVisible(), "Main content area should be visible on the home page");
        Assert.True(await _homePage.IsFooterVisible(), "Footer should be visible on the home page");
        Assert.True(await _homePage.IsBrandLogoVisible(), "CTF Arena logo should be visible on the home page");
    }

    [Trait("Category", "Smoke_UI")]
    [Fact]
    public async Task ShouldLoginWithValidCredentials()
    {
        // Navigate to sign in page and login
        var signInPage = await _homePage.GoToSignInPage();
        _homePage = await signInPage.SignIn(EnvironmentFixture.Configuration.WebServerCredentials.Username, EnvironmentFixture.Configuration.WebServerCredentials.Password);

        // Verify that the user's email is displayed in the navigation
        await _homePage.IsUserLoggedIn(EnvironmentFixture.Configuration.WebServerCredentials.Username);
    }
}