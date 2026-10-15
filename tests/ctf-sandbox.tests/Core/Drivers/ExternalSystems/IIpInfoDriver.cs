namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public interface IIpInfoDriver
{
    Task<Result<VoidValue, SystemError>> GoToIpInfo();
}