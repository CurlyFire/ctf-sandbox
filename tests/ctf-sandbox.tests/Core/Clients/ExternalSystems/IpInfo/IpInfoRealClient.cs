namespace ctf_sandbox.tests.Core.Clients.ExternalSystems.IpInfo;

public class IpInfoRealClient : HealthyTcpClient
{
    public IpInfoRealClient(HttpClient client)
        : base(client.BaseAddress!)
    {
    }
}