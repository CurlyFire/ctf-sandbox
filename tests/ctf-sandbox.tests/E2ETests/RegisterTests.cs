using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class RegisterTests : CTFTests
{
    public RegisterTests(RealExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToRegister(Channel channel)
    {
        var ctf = InteractWithCTFThrough(channel);
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";

        await ctf.CreateAccount(randomEmail, password);
        var emails = await ctf.CheckEmails();
        await emails.ActivateRegistrationSentTo(randomEmail);
        await ctf.SignIn(credentials =>
        {
            credentials.UserName = randomEmail;
            credentials.Password = password;
        });

        await ctf.ConfirmUserIsSignedIn(randomEmail);
    }
}
