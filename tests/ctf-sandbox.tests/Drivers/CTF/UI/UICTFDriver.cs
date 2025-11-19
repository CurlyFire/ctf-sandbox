using ctf_sandbox.Models;
using ctf_sandbox.tests.Drivers.CTF.UI.PageObjectModels;

namespace ctf_sandbox.tests.Drivers.CTF.UI;

public class UICTFDriver : ICTFDriver
{
    private readonly HomePage _homePage;
    public UICTFDriver(HomePage homePage)
    {
        _homePage = homePage;
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        var createAccountPage = await _homePage.GoToCreateAccountPage();
        await createAccountPage.FillEmail(email);
        await createAccountPage.FillPassword(password);
        await createAccountPage.FillConfirmPassword(password);
        var accountCreationConfirmationPage = await createAccountPage.CreateAccount();
        return await accountCreationConfirmationPage.IsConfirmationMessageVisible();
    }

    public async Task SignIn(string email, string password)
    {
        var signInPage = await _homePage.GoToSignInPage();
        await signInPage.SignIn(email, password);
    }

    public async Task<string?> CreateTeam(string? teamName)
    {
        var manageTeamsPage = await _homePage.GoToManageTeamsPage();
        var createNewTeamPage = await manageTeamsPage.GoToCreateNewTeamPage();
        return await createNewTeamPage.CreateTeam(teamName);
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null)
    {
        var manageTeamsPage = await _homePage.GoToManageTeamsPage();
        var editTeamPage = await manageTeamsPage.GoToEditTeamPage(oldTeamName);
        await editTeamPage.UpdateTeam(newTeamName, newDescription);
    }

    public async Task<bool> IsTeamAvailable(string teamName)
    {
        var manageTeamsPage = await _homePage.GoToManageTeamsPage();
        return await manageTeamsPage.IsTeamVisible(teamName);
    }

    public async Task<bool> IsUserSignedIn(string email)
    {
        return await _homePage.IsUserLoggedIn(email);
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        var ipInfoPage = await _homePage.GoToIpInfoPage();
        return await ipInfoPage.GetIpInfo(ipAddress);
    }
}
