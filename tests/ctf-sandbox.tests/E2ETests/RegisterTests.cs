using ctf_sandbox.tests.Fixture;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class RegisterTests : IClassFixture<DSLFixture>
{
    private readonly DSLFixture _fixture;

    public RegisterTests(DSLFixture fixture)
    {
        _fixture = fixture;
    }


    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToRegister(Channel channel)
    {
        var ctfDsl = _fixture.GetDsl(channel);

        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";
        Assert.True(await ctfDsl.CreateAccount(randomEmail, password));

        var emailsDsl = await ctfDsl.CheckEmails();
        Assert.True(await emailsDsl.ConfirmRegistrationSentTo(randomEmail));
    }
}
