using ctf_sandbox.tests.Core.Clients.ExternalSystems;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class IpInfoRealDriver : IIpInfoDriver
{
    private readonly IpInfoRealClient _ipInfoRealClient;

    public IpInfoRealDriver(IpInfoRealClient ipInfoRealClient)
    {
        _ipInfoRealClient = ipInfoRealClient;
    }

    public async Task<Result<VoidValue, SystemError>> GoToIpInfo()
    {
        var isHealthy = await _ipInfoRealClient.IsHealthy();
        if (isHealthy)
        {
            return Result.Success<SystemError>();
        }
        else
        {
            return Result.Failure<SystemError>(SystemError.Of("IpInfo service is not healthy"));
        }
    }
}
