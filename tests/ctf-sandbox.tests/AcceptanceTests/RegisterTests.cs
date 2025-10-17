using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.AcceptanceTests;

[Collection(StubbedExternalSystemsTestCollection.Name)]
public class RegisterTests : CTFTests
{
    public RegisterTests(StubbedExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "Acceptance")]
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
