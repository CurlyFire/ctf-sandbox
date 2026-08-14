using ctf_sandbox.tests.Core.Clients.ExternalSystems;
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
        var mailpitClient = new MailpitRealClient(new HttpClient { BaseAddress = new Uri(_fixture.Configuration!.MailpitUrl) });

        Assert.True(await mailpitClient.IsHealthy());
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task IpInfo_ShouldBeUpAndRunning()
    {
        var ipInfoClient = new IpInfoRealClient(new HttpClient { BaseAddress = new Uri(_fixture.Configuration!.IpInfoUrl) });

        Assert.True(await ipInfoClient.IsHealthy());
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task BannedWordsApi_ShouldBeUpAndRunning()
    {
        var bannedWordsClient = new BannedWordsRealClient(new HttpClient { BaseAddress = new Uri(_fixture.Configuration!.BannedWordsUrl) });

        Assert.True(await bannedWordsClient.IsHealthy());
    }
}
