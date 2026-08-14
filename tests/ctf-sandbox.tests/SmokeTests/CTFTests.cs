using ctf_sandbox.tests;
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
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.ConfirmIsUpAndRunning();
    }

    [Trait("Category", "Smoke_CTF")]
    [Theory]
    [Channel(Channel.UI, Channel.API)] 
    public async Task ShouldLoginWithValidCredentials(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();
        await ctf.ConfirmUserIsSignedIn();
    }
}