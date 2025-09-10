namespace ctf_sandbox.tests.E2ETests;

public class RegisterTests : WebServerPageTest
{
    public RegisterTests(WebServer webServer) : base(webServer)
    {
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ShouldBeAbleToRegister()
    {
        var homePage = await GoToHomePage();
        var createAccountPage = await homePage.GoToCreateAccountPage();
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";
        await createAccountPage.FillEmail(randomEmail);
        await createAccountPage.FillPassword(password);
        await createAccountPage.FillConfirmPassword(password);
        var accountCreationConfirmationPage = await createAccountPage.CreateAccount();
        Assert.True(await accountCreationConfirmationPage.IsConfirmationMessageVisible());

        var emailsPage = await homePage.GoToEmailsPage();
        var confirmEmailPage = await emailsPage.OpenLatestEmailSentToAndOpenConfirmationLink(randomEmail);
        Assert.True(await confirmEmailPage.IsThankYouMessageVisible());
    }
}
