using RestEase;
using WireMock.Client;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class ClockStubbedDriver : IClockDriver
{
    public ClockStubbedDriver(IWireMockAdminApi api)
    {
    }

    public Task<Result<VoidValue, SystemError>> GoToClock()
    {
        return Task.FromResult(Result.Success<SystemError>());
    }
}