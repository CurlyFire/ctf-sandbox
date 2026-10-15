namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class ClockRealClient
{
    private readonly TimeProvider _timeProvider;

    public ClockRealClient()
    {
        _timeProvider = TimeProvider.System;
    }

    public DateTimeOffset GetCurrentTime() => _timeProvider.GetUtcNow();
}
