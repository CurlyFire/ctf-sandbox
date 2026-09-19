namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class IpInfoRealClient : HealthyTcpClient
{
    public IpInfoRealClient(HttpClient client)
        : base(client.BaseAddress!)
    {
    }
}