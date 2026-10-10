using ctf_sandbox.tests;
using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace tests.ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class CTFTests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public CTFTests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "Smoke_CTF")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]    
    public async Task ShouldBeUpAndRunning(Channel channel)
    {
        await _fixture.Scenario(channel)
            .Assume().MyShop().ShouldBeRunning();
    }
}