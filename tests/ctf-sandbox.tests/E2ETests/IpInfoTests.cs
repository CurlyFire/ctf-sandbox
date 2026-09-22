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
        var ctf = _fixture.InteractWithSystemThrough(channel).CTF;
        (await ctf.SignIn().Execute()).ShouldSucceed();

        (await ctf.GetIpInfo().With(ip => ip.IpAddress = "8.8.8.8").Execute())
            .ShouldSucceed()
            .HasIp("8.8.8.8")
            .HasLocationData();
    }
}
