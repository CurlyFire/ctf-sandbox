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
        var system = _fixture.InteractWithSystemThrough(channel);
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";

        (await system.CTF.CreateAccount().With(account =>
        {
            account.Email = randomEmail;
            account.Password = password;
        }).Execute()).ShouldSucceed();
        

        var emails = system.ExternalSystems.Emails;
        await emails.ActivateRegistrationSentTo(randomEmail);
        (await system.CTF.SignIn().With(credentials =>
        {
            credentials.UserName = randomEmail;
            credentials.Password = password;
        }).Execute()).ShouldSucceed();
    }
}
