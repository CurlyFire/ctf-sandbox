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
    [Channel(Channel.API)]
    public async Task ShouldBeAbleToGetIpInfo(Channel channel)
    {
    }
}
