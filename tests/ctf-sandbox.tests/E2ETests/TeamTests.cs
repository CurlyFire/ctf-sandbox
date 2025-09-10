namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : WebServerPageTest
{
    public TeamTests(WebServer webServer) : base(webServer)
    {
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ShouldBeAbleToCreateTeam()
    {
        var homePage = await GoToHomePage();
        var signInPage = await homePage.GoToSignInPage();
        await signInPage.SignIn(WebServer.WebServerCredentials.Username, WebServer.WebServerCredentials.Password);
        var manageTeamsPage = await homePage.GoToManageTeamsPage();
        var createNewTeamPage = await manageTeamsPage.GoToCreateNewTeamPage();
        var randomTeamName = $"team_{Guid.NewGuid()}";
        await createNewTeamPage.CreateTeam(randomTeamName);
        Assert.True(await manageTeamsPage.IsTeamVisible(randomTeamName));
    }
}
