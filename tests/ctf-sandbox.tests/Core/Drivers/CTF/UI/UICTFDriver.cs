using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Clients.UI;

namespace ctf_sandbox.tests.Core.Drivers.CTF.UI;

public class UICTFDriver : ICTFDriver
{
    private readonly UIClient _uiClient;
    public UICTFDriver(UIClient uiClient)
    {
        _uiClient = uiClient;
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        var homePage = await _uiClient.OpenHomePage();
        var createAccountPage = await homePage.GoToCreateAccountPage();
        await createAccountPage.FillEmail(email);
        await createAccountPage.FillPassword(password);
        await createAccountPage.FillConfirmPassword(password);
        var accountCreationConfirmationPage = await createAccountPage.CreateAccount();
        return await accountCreationConfirmationPage.IsConfirmationMessageVisible();
    }

    public async Task SignIn(string email, string password)
    {
        var homePage = await _uiClient.OpenHomePage();
        var signInPage = await homePage.GoToSignInPage();
        await signInPage.SignIn(email, password);
    }

    public async Task<string?> CreateTeam(string? teamName, uint memberCount = 4)
    {
        var homePage = await _uiClient.OpenHomePage();
        var manageTeamsPage = await homePage.GoToManageTeamsPage();
        var createNewTeamPage = await manageTeamsPage.GoToCreateNewTeamPage();
        return await createNewTeamPage.CreateTeam(teamName, memberCount);
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        var homePage = await _uiClient.OpenHomePage();
        var manageTeamsPage = await homePage.GoToManageTeamsPage();
        var editTeamPage = await manageTeamsPage.GoToEditTeamPage(oldTeamName);
        await editTeamPage.UpdateTeam(newTeamName, newDescription, memberCount);
    }

    public async Task<Team?> GetTeam(string teamName)
    {
        var homePage = await _uiClient.OpenHomePage();
        var manageTeamsPage = await homePage.GoToManageTeamsPage();
        return await manageTeamsPage.GetTeam(teamName);
    }

    public async Task ConfirmUserIsSignedIn(string email)
    {
        var homePage = await _uiClient.OpenHomePage();
        Assert.True(await homePage.IsUserLoggedIn(email));
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        var homePage = await _uiClient.OpenHomePage();
        var ipInfoPage = await homePage.GoToIpInfoPage();
        return await ipInfoPage.GetIpInfo(ipAddress);
    }

    public async Task ConfirmIsUpAndRunning()
    {
        var homePage = await _uiClient.OpenHomePage();
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
}
