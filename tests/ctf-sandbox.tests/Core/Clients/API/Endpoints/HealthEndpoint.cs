namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class HealthEndpoint : Endpoint
{
    public HealthEndpoint(HttpClient httpClient) : base(httpClient)
    {

    }
    public async Task<bool> IsHealthy()
    {
        try
        {
            var healthResponse = await HttpClient.GetAsync("/health");
            var healthReadyResponse = await HttpClient.GetAsync("/health/ready");
            var healthLiveResponse = await HttpClient.GetAsync("/health/live");
            return healthResponse.IsSuccessStatusCode && healthReadyResponse.IsSuccessStatusCode && healthLiveResponse.IsSuccessStatusCode;
        }
        catch (UnsuccessfulHttpResponseException)
        {
            return false;
        }
    }
}