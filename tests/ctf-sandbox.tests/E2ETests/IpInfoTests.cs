using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class IpInfoTests : CTFTests
{
    public IpInfoTests(RealExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToGetIpInfo(Channel channel)
    {
        var ctf = InteractWithCTFThrough(channel);
        await ctf.SignIn();

        var ipInfo = await ctf.GetIpInfo("8.8.8.8");

        Assert.Equal("8.8.8.8", ipInfo.Ip);
        Assert.Equal("dns.google", ipInfo.Hostname);
        Assert.Equal("Mountain View", ipInfo.City);
        Assert.Equal("California", ipInfo.Region);
        Assert.Equal("US", ipInfo.Country);
    }
}
