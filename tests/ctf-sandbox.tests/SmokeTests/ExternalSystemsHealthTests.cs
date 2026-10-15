using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(CTFTestCollection.Name)]
public class ExternalSystemsHealthTests
{
    private readonly TestEnvironments _testEnvironments;

    public ExternalSystemsHealthTests(TestEnvironments testEnvironments)
    {
        _testEnvironments = testEnvironments;
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task Mailpit_ShouldBeUpAndRunning(Channel channel, ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario(channel)
            .Assume().Mailpit().ShouldBeRunning();
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task IpInfo_ShouldBeUpAndRunning(Channel channel, ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario(channel)
            .Assume().IpInfo().ShouldBeRunning();
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task Clock_ShouldBeUpAndRunning(Channel channel, ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario(channel)
            .Assume().Clock().ShouldBeRunning();
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task BannedWordsApi_ShouldBeUpAndRunning(Channel channel, ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario(channel)
            .Assume().BannedWords().ShouldBeRunning();
    }
}
