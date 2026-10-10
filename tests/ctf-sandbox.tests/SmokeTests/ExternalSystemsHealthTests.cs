using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class ExternalSystemsHealthTests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public ExternalSystemsHealthTests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Fact]
    public async Task Mailpit_ShouldBeUpAndRunning()
    {
        await _fixture.Scenario()
            .Assume().Mailpit().ShouldBeRunning();
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task IpInfo_ShouldBeUpAndRunning()
    {
        await _fixture.Scenario()
            .Assume().IpInfo().ShouldBeRunning();
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task BannedWordsApi_ShouldBeUpAndRunning()
    {
        await _fixture.Scenario()
            .Assume().BannedWords().ShouldBeRunning();
    }
}
