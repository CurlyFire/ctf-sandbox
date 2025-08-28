namespace ctf_sandbox.tests.SmokeTests;

public class FrontEndHealthTests : IClassFixture<WebServer>
{
    private readonly WebServer _webServer;

    public FrontEndHealthTests(WebServer webServer)
    {
        _webServer = webServer;
    }

    [Trait("Category", "Smoke_FrontEndHealth")]
    [Fact]
    public async Task FrontEnd_ShouldBeUpAndRunning()
    {
        using var client = new HttpClient()
        {
            BaseAddress = new Uri(_webServer.WebServerUrl!)
        };
        
        // Check main health endpoint
        var response = await client.GetAsync($"/health");
        Assert.True(response.IsSuccessStatusCode, $"Health check failed at {_webServer.WebServerUrl}/health - status code: {response.StatusCode}");
        
        // Check readiness probe
        response = await client.GetAsync($"/health/ready");
        Assert.True(response.IsSuccessStatusCode, $"Readiness check failed at {_webServer.WebServerUrl}/health/ready - status code: {response.StatusCode}");
        
        // Check liveness probe
        response = await client.GetAsync($"/health/live");
        Assert.True(response.IsSuccessStatusCode, $"Liveness check failed at {_webServer.WebServerUrl}/health/live - status code: {response.StatusCode}");
    }
}
