namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public interface IClockDriver
{
    Task<Result<VoidValue, SystemError>> GoToClock();
}
