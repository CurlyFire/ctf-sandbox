// namespace ctf_sandbox.tests.SmokeTests;

// public class UITests : WebServerPageTest
// {
//     public UITests(WebServer webServer) : base(webServer)
//     {
//     }

//     [Trait("Category", "Smoke_UI")]
//     [Fact]
//     public async Task ShouldLoadMainPage()
//     {
//         var homePage = await GoToHomePage();

//         // Check if the page title is correct
//         var title = await homePage.GetPageTitle();
//         Assert.Equal("Home Page - CTF Arena", title);

//         // Verify each main layout component individually
//         Assert.True(await homePage.IsBannerVisible(), "Header banner should be visible on the home page");
//         Assert.True(await homePage.IsMainNavigationVisible(), "Main navigation menu should be visible on the home page");
//         Assert.True(await homePage.IsDashboardLinkVisible(), "Dashboard link should be visible on the home page");
//         Assert.True(await homePage.IsMainContentAreaVisible(), "Main content area should be visible on the home page");
//         Assert.True(await homePage.IsFooterVisible(), "Footer should be visible on the home page");
//         Assert.True(await homePage.IsBrandLogoVisible(), "CTF Arena logo should be visible on the home page");
//     }

//     [Trait("Category", "Smoke_UI")]
//     [Fact]
//     public async Task ShouldLoginWithValidCredentials()
//     {
//         // Start from home page
//         var homePage = await GoToHomePage();

//         // Navigate to sign in page and login
//         var signInPage = await homePage.GoToSignInPage();
//         homePage = await signInPage.SignIn(WebServer.WebServerCredentials.Username, WebServer.WebServerCredentials.Password);

//         // Verify that the user's email is displayed in the navigation
//         var username = await homePage.GetLoggedInUsername();
//         Assert.Contains(WebServer.WebServerCredentials.Username, username);
//     }
// }