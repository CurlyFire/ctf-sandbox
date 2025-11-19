using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class IpInfoTests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public IpInfoTests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToGetIpInfo(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();

        var ipInfo = await ctf.GetIpInfo("8.8.8.8");

        // Limited assertions as we don't control the data returned by the external service
        Assert.Equal("8.8.8.8", ipInfo.Ip);
        Assert.NotEmpty(ipInfo.Hostname);
        Assert.NotEmpty(ipInfo.City);
        Assert.NotEmpty(ipInfo.Region);
        Assert.NotEmpty(ipInfo.Country);
        Assert.NotEmpty(ipInfo.Timezone);
    }
}
