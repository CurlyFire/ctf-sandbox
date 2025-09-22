namespace ctf_sandbox.tests.E2ETests;

public class RegisterTests : CTFDslTests
{

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ShouldBeAbleToRegister()
    {
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";

        Assert.True(await CTFDsl.CreateAccount(randomEmail, password));

        var emailsDsl = await CTFDsl.CheckEmails();
        Assert.True(await emailsDsl.ConfirmRegistrationSentTo(randomEmail));
    }
}
