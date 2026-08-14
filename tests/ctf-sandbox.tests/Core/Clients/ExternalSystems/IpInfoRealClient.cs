namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class IpInfoRealClient : HealthyHttpClient
{
    public IpInfoRealClient(HttpClient client) : base(client)
    {
    }
}