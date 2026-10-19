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
    public async Task Mailpit_ShouldBeUpAndRunning(ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario()
            .Assume().Mailpit().ShouldBeRunning();
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task IpInfo_ShouldBeUpAndRunning(ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario()
            .Assume().IpInfo().ShouldBeRunning();
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task Clock_ShouldBeUpAndRunning(ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario()
            .Assume().Clock().ShouldBeRunning();
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Theory, CombinatorialData]
    public async Task BannedWordsApi_ShouldBeUpAndRunning(ExternalSystems externalSystem)
    {
        await _testEnvironments.For(externalSystem).Scenario()
            .Assume().BannedWords().ShouldBeRunning();
    }
}
