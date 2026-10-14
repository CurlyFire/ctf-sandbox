using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(CTFTestCollection.Name)]
public class ExternalSystemsHealthTests
{
    private readonly TestEnvironment _testEnvironment;

    public ExternalSystemsHealthTests(TestEnvironments testEnvironments)
    {
        _testEnvironment = testEnvironments.For(ExternalSystem.Real);
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Fact]
    public async Task Mailpit_ShouldBeUpAndRunning()
    {
        await _testEnvironment.Scenario()
            .Assume().Mailpit().ShouldBeRunning();
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task IpInfo_ShouldBeUpAndRunning()
    {
        await _testEnvironment.Scenario()
            .Assume().IpInfo().ShouldBeRunning();
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task BannedWordsApi_ShouldBeUpAndRunning()
    {
        await _testEnvironment.Scenario()
            .Assume().BannedWords().ShouldBeRunning();
    }
}
