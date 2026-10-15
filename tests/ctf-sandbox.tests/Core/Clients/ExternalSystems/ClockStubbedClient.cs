namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class ClockStubbedClient : HealthyTcpClient
{
    private readonly HttpClient _httpClient;

    public ClockStubbedClient(HttpClient httpClient) : base(httpClient.BaseAddress!)
    {
        _httpClient = httpClient;
    }
}