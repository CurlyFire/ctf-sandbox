using ctf_sandbox.tests;
using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace tests.ctf_sandbox.tests.SmokeTests;

[Collection(CTFTestCollection.Name)]
public class CTFTests
{
    private readonly TestEnvironments _testEnvironments;

    public CTFTests(TestEnvironments testEnvironments)
    {
        _testEnvironments = testEnvironments;
    }

    [Trait("Category", "Smoke_CTF")]
    [Theory, CombinatorialData]
    public async Task ShouldBeUpAndRunning(Channel channel, ExternalSystem externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario(channel)
            .Assume().MyShop().ShouldBeRunning();
    }
}