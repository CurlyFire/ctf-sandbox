using ctf_sandbox.tests.Core.Dsl.Scenario;
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
        var randomEmail = $"registertest_{Guid.NewGuid()}@test.com";
        var password = "RegisterTest123!";

        await _fixture.Scenario(channel)
            .Given()
                .NotSignedIn()
            .And()
                .Account()
                .WithEmail(randomEmail)
                .WithPassword(password)
            .When()
                .SignIn()
                    .WithUserName(randomEmail)
                    .WithPassword(password)
            .Then()
                .ShouldSucceed();
    }
}
