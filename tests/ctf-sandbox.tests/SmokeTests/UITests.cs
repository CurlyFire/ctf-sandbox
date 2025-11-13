using ctf_sandbox.tests.Drivers.UI.PageObjectModels;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class UITests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public UITests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "Smoke_UI")]
    [Fact]
    public async Task ShouldLoadMainPage()
    {
        var homePage = _fixture.GetNewHomePage();

        // Check if the page title is correct
        var title = await homePage.GetPageTitle();
        Assert.Equal("Home Page - CTF Arena", title);

        // Verify each main layout component individually
        Assert.True(await homePage.IsBannerVisible(), "Header banner should be visible on the home page");
        Assert.True(await homePage.IsMainNavigationVisible(), "Main navigation menu should be visible on the home page");
        Assert.True(await homePage.IsDashboardLinkVisible(), "Dashboard link should be visible on the home page");
        Assert.True(await homePage.IsMainContentAreaVisible(), "Main content area should be visible on the home page");
        Assert.True(await homePage.IsFooterVisible(), "Footer should be visible on the home page");
        Assert.True(await homePage.IsBrandLogoVisible(), "CTF Arena logo should be visible on the home page");
    }

    [Trait("Category", "Smoke_UI")]
    [Fact]
    public async Task ShouldLoginWithValidCredentials()
    {
        var homePage = _fixture.GetNewHomePage();
        // Navigate to sign in page and login
        var signInPage = await homePage.GoToSignInPage();
        homePage = await signInPage.SignIn(_fixture.Configuration.WebServerCredentials.Username, _fixture.Configuration.WebServerCredentials.Password);

        // Verify that the user's email is displayed in the navigation
        await homePage.IsUserLoggedIn(_fixture.Configuration.WebServerCredentials.Username);
    }
}