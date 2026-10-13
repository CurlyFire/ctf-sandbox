using ctf_sandbox.tests.Core.Dsl.Scenario;
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
        await _fixture.Scenario(channel)
            .Given().SignedIn()
            .When().GetIpInfo().WithIpAddress("8.8.8.8")
            .Then().ShouldSucceed().And().Response(verification => verification
                .HasIp("8.8.8.8")
                .HasLocationData());
    }
}
