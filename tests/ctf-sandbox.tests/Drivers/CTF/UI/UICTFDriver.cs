using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Clients.UI;

namespace ctf_sandbox.tests.Drivers.CTF.UI;

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

    public async Task<bool> IsUserSignedIn(string email)
    {
        var homePage = await _uiClient.OpenHomePage();
        return await homePage.IsUserLoggedIn(email);
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        var homePage = await _uiClient.OpenHomePage();
        var ipInfoPage = await homePage.GoToIpInfoPage();
        return await ipInfoPage.GetIpInfo(ipAddress);
    }
}
