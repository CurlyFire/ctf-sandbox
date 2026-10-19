using ctf_sandbox.tests.Core.Clients.ExternalSystems.IpInfo;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class IpInfoStubbedDriver : IIpInfoDriver
{
    private readonly IpInfoStubbedClient _ipInfoStubbedClient;

    public IpInfoStubbedDriver(IpInfoStubbedClient ipInfoStubbedClient)
    {
        _ipInfoStubbedClient = ipInfoStubbedClient;
    }

    public async Task<Result<VoidValue, SystemError>> GoToIpInfo()
    {
        return await _ipInfoStubbedClient.CheckHealth();
    }
}