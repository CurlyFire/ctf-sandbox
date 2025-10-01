using ctf_sandbox.tests.Fixtures.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class RegisterTests : CTFTests
{
    public RegisterTests(ServerFixture fixture) : base(fixture)
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
        await emails.ConfirmRegistrationSentTo(randomEmail);
    }
}
