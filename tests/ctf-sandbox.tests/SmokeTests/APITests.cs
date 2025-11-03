using System.Net.Http.Json;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class APITests : HttpTests
{
    private HttpClient _httpClient;

    public APITests(RealExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
        _httpClient = GetCTFHttpClient();
    }

    [Trait("Category", "Smoke_API")]
    [Fact]
    public async Task ShouldLoginWithValidCredentials()
    {
        var response = await _httpClient.PostAsJsonAsync("auth",
            new LoginRequest()
            {
                Username = EnvironmentFixture.Configuration.WebServerCredentials.Username,
                Password = EnvironmentFixture.Configuration.WebServerCredentials.Password
            });

        response.EnsureSuccessStatusCode();
    }
}