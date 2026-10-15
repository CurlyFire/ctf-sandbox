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
        await _fixture.Scenario(channel)
            .Given()
                .NotSignedIn()
            .And()
                .Account()
                .WithEmail("registertest@test.com")
                .WithPassword("RegisterTest123!")
            .When()
                .SignIn()
                    .WithUserName("registertest@test.com")
                    .WithPassword("RegisterTest123!")
            .Then()
                .ShouldSucceed();
    }
}
