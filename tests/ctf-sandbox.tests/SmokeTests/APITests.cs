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
        var client = _fixture.InteractWithCTFThroughHttpClient();
        var response = await client.PostAsJsonAsync("auth",
            new LoginRequest()
            {
                Username = _fixture.Configuration.WebServerCredentials.Username,
                Password = _fixture.Configuration.WebServerCredentials.Password
            });

        response.EnsureSuccessStatusCode();
    }
}