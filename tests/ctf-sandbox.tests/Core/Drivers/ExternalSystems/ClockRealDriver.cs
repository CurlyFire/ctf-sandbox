using ctf_sandbox.tests.Core.Clients.ExternalSystems;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class ClockRealDriver : IClockDriver
{
    private readonly ClockRealClient _clockRealClient;

    public ClockRealDriver(ClockRealClient clockRealClient)
    {
        _clockRealClient = clockRealClient;
    }

    public Task<Result<VoidValue, SystemError>> GoToClock()
    {
        _ = _clockRealClient.GetCurrentTime();
        return Task.FromResult(Result.Success<SystemError>());
    }
}
