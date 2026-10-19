using ctf_sandbox.tests.Core.Clients.ExternalSystems.Clock;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class ClockStubbedDriver : IClockDriver
{
    private readonly ClockStubbedClient _clockStubbedClient;

    public ClockStubbedDriver(ClockStubbedClient clockStubbedClient)
    {
        _clockStubbedClient = clockStubbedClient;
    }

    public Task<Result<VoidValue, SystemError>> GoToClock() => _clockStubbedClient.CheckHealth();
}
