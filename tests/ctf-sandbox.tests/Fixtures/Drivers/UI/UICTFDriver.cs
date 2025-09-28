using ctf_sandbox.tests.Fixture.Drivers.UI.PageObjectModels;
using ctf_sandbox.tests.Fixture.Dsl;
namespace ctf_sandbox.tests.Fixture.Drivers.UI;

public class UICTFDriver : ICTFDriver
{
    private readonly HomePage _homePage;
    public UICTFDriver(HomePage homePage)
    {
        _homePage = homePage;
    }

    public async Task<EmailsDsl> CheckEmails()
    {
        var emailsPage = await _homePage.GoToEmailsPage();
        var uiEmailsDriver = new UIEmailsDriver(emailsPage);
        return new EmailsDsl(uiEmailsDriver);
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

    public async Task CreateTeam(string teamName)
    {
        var manageTeamsPage = await _homePage.GoToManageTeamsPage();
        var createNewTeamPage = await manageTeamsPage.GoToCreateNewTeamPage();
        await createNewTeamPage.CreateTeam(teamName);
    }

    public async Task<bool> IsTeamVisible(string teamName)
    {
        var manageTeamsPage = await _homePage.GoToManageTeamsPage();
        return await manageTeamsPage.IsTeamVisible(teamName);
    }
}
