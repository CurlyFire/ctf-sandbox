namespace ctf_sandbox.tests.Clients.ExternalSystems;

public class IpInfoRealClient : HealthyHttpClient
{
    public IpInfoRealClient(HttpClient client) : base(client)
    {
    }
}