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
        var system = _fixture.InteractWithSystem();
        (await system.ExternalSystems.Emails.GoToMailpit().Execute()).ShouldSucceed();
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task IpInfo_ShouldBeUpAndRunning()
    {
        var system = _fixture.InteractWithSystem();
        (await system.ExternalSystems.IpInfo.GoToIpInfo().Execute()).ShouldSucceed();
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task BannedWordsApi_ShouldBeUpAndRunning()
    {
        var system = _fixture.InteractWithSystem();
        (await system.ExternalSystems.BannedWords.GoToBannedWords().Execute()).ShouldSucceed();
    }
}
