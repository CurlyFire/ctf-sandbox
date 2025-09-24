using ctf_sandbox.tests.Dsl;
using ctf_sandbox.tests.PageObjectModels;
using Microsoft.Playwright;
using Microsoft.Playwright.TestAdapter;

namespace ctf_sandbox.tests.Drivers;

public class UICTFDriver : ICTFDriver
{
    private readonly HomePage _homePage;
    private readonly IPlaywright _playwright;
    private bool disposedValue;

    public UICTFDriver(string baseUrl)
    {
        _playwright = Playwright.CreateAsync().Result;
        var browserType = _playwright[PlaywrightSettingsProvider.BrowserName];
        var browser = browserType.LaunchAsync().Result;
        var options = new BrowserNewPageOptions
        {
            BaseURL = baseUrl
        };
        var page = browser.NewPageAsync(options).Result;
        page.GotoAsync(string.Empty).Wait();
        _homePage = new HomePage(page);
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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _playwright.Dispose();
            }
            disposedValue = true;
        }
    }


    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
