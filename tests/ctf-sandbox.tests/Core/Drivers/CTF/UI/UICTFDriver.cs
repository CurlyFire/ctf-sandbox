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

    public async Task<Result<VoidValue, SystemError>> CreateAccount(string email, string password)
    {
        var homePage = await _uiClient.OpenHomePage();
        var createAccountPage = await homePage.GoToCreateAccountPage();
        await createAccountPage.FillEmail(email);
        await createAccountPage.FillPassword(password);
        await createAccountPage.FillConfirmPassword(password);
        var accountCreationConfirmationPage = await createAccountPage.CreateAccount();
        var errors = await accountCreationConfirmationPage.GetErrors();
        if (errors != null)
        {
            return Result.Failure(ValidationProblemDetailsExtensions.MapError(errors));
        }
        else
        {
            var result = await accountCreationConfirmationPage.IsConfirmationMessageVisible();
            return result ? Result.Success<SystemError>() : Result.Failure(SystemError.Of("Account creation confirmation message was not found"));
        }
    }

    public async Task<Result<VoidValue, SystemError>> SignIn(string? email, string? password)
    {
        var homePage = await _uiClient.OpenHomePage();
        var signInPage = await homePage.GoToSignInPage();
        var result = await signInPage.SignIn(email, password);
        if (result.IsSuccess)
        {
            return Result.Success<SystemError>();
        }
        else
        {
            return Result.Failure(ValidationProblemDetailsExtensions.MapError(result.Error));
        }
    }

    public async Task<Result<Team?, SystemError>> CreateTeam(string? teamName, uint memberCount = 4)
    {
        var homePage = await _uiClient.OpenHomePage();
        var manageTeamsPage = await homePage.GoToManageTeamsPage();
        var createNewTeamPage = await manageTeamsPage.GoToCreateNewTeamPage();
        var result = await createNewTeamPage.CreateTeam(teamName, memberCount);
        if (result.IsSuccess)
        {
            var createdTeam = await result.Value.GetTeam(teamName);
            if (createdTeam.IsSuccess)
                return Result<Team?, SystemError>.Success(createdTeam.Value);
            else
                return Result<Team?, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(createdTeam.Error));
        }
        else
        {
            return Result<Team?, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(result.Error));
        }        
    }

    public async Task<Result<Team, SystemError>> UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        var homePage = await _uiClient.OpenHomePage();
        var manageTeamsPage = await homePage.GoToManageTeamsPage();
        var editTeamPage = await manageTeamsPage.GoToEditTeamPage(oldTeamName);
        var result = await editTeamPage.UpdateTeam(newTeamName, newDescription, memberCount);
        if (result.IsSuccess)
        {
            var team = await result.Value.GetTeam(newTeamName);
            return Result<Team, SystemError>.Success(team.Value);
        }
        else
        {
            return Result<Team, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(result.Error));
        }
    }

    public async Task<Result<Team, SystemError>> GetTeam(string teamName)
    {
        var homePage = await _uiClient.OpenHomePage();
        var manageTeamsPage = await homePage.GoToManageTeamsPage();

        var result = await manageTeamsPage.GetTeam(teamName);
        if (result.IsSuccess)
        {
            return Result<Team, SystemError>.Success(result.Value);
        }
        else
        {
            return Result<Team, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(result.Error));
        }
    }

    public async Task<Result<VoidValue, SystemError>> ConfirmUserIsSignedIn(string email)
    {
        var homePage = await _uiClient.OpenHomePage();
        var isLoggedIn = await homePage.IsUserLoggedIn(email);
        if (isLoggedIn)
            return Result.Success<SystemError>();
        else
            return Result<VoidValue, SystemError>.Failure(SystemError.Of("User is not signed in"));
    }

    public async Task<Result<IpInfo, SystemError>> GetIpInfo(string ipAddress)
    {
        var homePage = await _uiClient.OpenHomePage();
        var ipInfoPage = await homePage.GoToIpInfoPage();
        var ipInfo = await ipInfoPage.GetIpInfo(ipAddress);
        var errors = await ipInfoPage.GetErrors();
        if (errors != null)
        {
            return Result<IpInfo, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(errors));
        }
        return Result<IpInfo, SystemError>.Success(ipInfo);
    }

    public async Task<Result<VoidValue, SystemError>> ConfirmIsUpAndRunning()
    {
        var homePage = await _uiClient.OpenHomePage();
        var title = await homePage.GetPageTitle();
        if (title != "Home Page - CTF Arena")
            return Result.Failure(SystemError.Of("The CTF Arena home page title is incorrect."));

        if (!await homePage.IsBannerVisible())
            return Result.Failure(SystemError.Of("The home page header banner is not visible."));

        if (!await homePage.IsMainNavigationVisible())
            return Result.Failure(SystemError.Of("The home page main navigation menu is not visible."));

        if (!await homePage.IsDashboardLinkVisible())
            return Result.Failure(SystemError.Of("The home page dashboard link is not visible."));

        if (!await homePage.IsMainContentAreaVisible())
            return Result.Failure(SystemError.Of("The home page main content area is not visible."));

        if (!await homePage.IsFooterVisible())
            return Result.Failure(SystemError.Of("The home page footer is not visible."));

        if (!await homePage.IsBrandLogoVisible())
            return Result.Failure(SystemError.Of("The CTF Arena logo is not visible on the home page."));

        return Result.Success<SystemError>();
    }

    public async Task<Result<VoidValue, SystemError>> GoToCTF()
    {
        var homePage = await _uiClient.OpenHomePage();
        var errors = await homePage.GetErrors();
        if (errors != null)
        {
            return Result.Failure(ValidationProblemDetailsExtensions.MapError(errors));
        }
        return Result.Success<SystemError>();
    }
}
