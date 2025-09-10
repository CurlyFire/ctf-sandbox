namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : WebServerPageTest
{
    public EmailSystemTests(WebServer webServer) : base(webServer)
    {
    }

    [Trait("Category", "E2E")]
    [Fact]
    public async Task ShouldBeAbleToViewEmails()
    {
        var homePage = await GoToHomePage();
        var emailsPage = await homePage.GoToEmailsPage();

        Assert.True(await emailsPage.IsInboxVisible());
    }
}

