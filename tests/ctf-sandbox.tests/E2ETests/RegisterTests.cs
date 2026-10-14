using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(CTFTestCollection.Name)]
public class RegisterTests
{
    private readonly TestEnvironment _testEnvironment;

    public RegisterTests(TestEnvironments testEnvironments)
    {
        _testEnvironment = testEnvironments.For(ExternalSystem.Real);
    }

    [Trait("Category", "E2E")]
    [Theory, CombinatorialData]
    public async Task ShouldBeAbleToRegister(Channel channel)
    {
        await _testEnvironment.Scenario(channel)
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
