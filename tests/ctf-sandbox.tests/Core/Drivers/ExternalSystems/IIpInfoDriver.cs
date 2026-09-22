using ctf_sandbox.tests.Core;

namespace CtfSandbox.Tests.Core.Drivers.ExternalSystems;

public interface IIpInfoDriver
{
    Task<Result<VoidValue, SystemError>> GoToIpInfo();
}