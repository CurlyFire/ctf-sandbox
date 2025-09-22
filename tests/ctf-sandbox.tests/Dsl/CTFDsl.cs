using ctf_sandbox.tests.PageObjectModels;
namespace ctf_sandbox.tests.Dsl;

public class CTFDsl
{
    private readonly HomePage _homePage;
    private Credentials _ctfAdminCredentials { get; set; }
    private Credentials _emailCredentials { get; set; }

    public CTFDsl(HomePage page, Credentials ctfAdminCredentials, Credentials emailCredentials)
    {
        _ctfAdminCredentials = ctfAdminCredentials;
        _emailCredentials = emailCredentials;
        _homePage = page;
    }

    public async Task<EmailsDsl> CheckEmails()
    {
        var emailsPage = await _homePage.GoToEmailsPage();
        return new EmailsDsl(emailsPage);
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

    public async Task<CTFDsl> SignIn(string email, string password)
    {
        var signInPage = await _homePage.GoToSignInPage();
        await signInPage.SignIn(email, password);
        return this;
    }

    public async Task<CTFDsl> SignInAsAdmin()
    {
        return await SignIn(_ctfAdminCredentials.Username, _ctfAdminCredentials.Password);
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
