using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class FrontEndHealthTests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public FrontEndHealthTests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "Smoke_FrontEndHealth")]
    [Fact]
    public async Task FrontEnd_ShouldBeUpAndRunning()
    {
        var config = _fixture.Configuration;
        using var client = new HttpClient()
        {
            BaseAddress = new Uri(config.WebServerUrl!)
        };

        // Check main health endpoint
        var response = await client.GetAsync($"/health");
        Assert.True(response.IsSuccessStatusCode, $"Health check failed at {config.WebServerUrl}/health - status code: {response.StatusCode}");

        // Check readiness probe
        response = await client.GetAsync($"/health/ready");
        Assert.True(response.IsSuccessStatusCode, $"Readiness check failed at {config.WebServerUrl}/health/ready - status code: {response.StatusCode}");

        // Check liveness probe
        response = await client.GetAsync($"/health/live");
        Assert.True(response.IsSuccessStatusCode, $"Liveness check failed at {config.WebServerUrl}/health/live - status code: {response.StatusCode}");
    }
}
