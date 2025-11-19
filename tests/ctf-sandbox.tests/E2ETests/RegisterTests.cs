using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class RegisterTests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public RegisterTests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToRegister(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";

        await ctf.CreateAccount(randomEmail, password);

        var emails = _fixture.ExternalSystems.InteractWithEmails();
        await emails.ActivateRegistrationSentTo(randomEmail);
        await ctf.SignIn(credentials =>
        {
            credentials.UserName = randomEmail;
            credentials.Password = password;
        });

        await ctf.ConfirmUserIsSignedIn(randomEmail);
    }
}
