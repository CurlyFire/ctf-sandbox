using System.Net.Http.Json;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class APITests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public APITests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "Smoke_API")]
    [Fact]
    public async Task ShouldLoginWithValidCredentials()
    {
        var authenticationEndpoint = _fixture.InteractWithCTFThroughAPIClient().Authentication;
        await  authenticationEndpoint.Authenticate(_fixture.Configuration.WebServerCredentials.Username,
            _fixture.Configuration.WebServerCredentials.Password);
    }
}