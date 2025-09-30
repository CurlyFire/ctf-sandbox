using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

public class FrontEndHealthTests : WebServerTests
{
    public FrontEndHealthTests(ServerFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "Smoke_FrontEndHealth")]
    [Fact]
    public async Task FrontEnd_ShouldBeUpAndRunning()
    {
        var config = ServerFixture.Configuration;
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
