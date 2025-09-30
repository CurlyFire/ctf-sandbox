using ctf_sandbox.tests.Fixtures.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class RegisterTests : DSLTests
{
    public RegisterTests(ServerFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToRegister(Channel channel)
    {
        var ctfDsl = GetDsl(channel);

        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";
        Assert.True(await ctfDsl.CreateAccount(randomEmail, password));

        var emailsDsl = await ctfDsl.CheckEmails();
        Assert.True(await emailsDsl.ConfirmRegistrationSentTo(randomEmail));
    }
}
