using ctf_sandbox.tests.Core.Dsl.UseCases;
using CtfSandbox.Tests.Core.Drivers.ExternalSystems;
using CtfSandbox.Tests.Core.Dsl.External.IpInfo.UseCases;

namespace CtfSandbox.Tests.Core.Dsl.External.IpInfo;

public class IpInfoDsl
{
    private readonly UseCaseFactory<IIpInfoDriver> _useCaseFactory;
    public IpInfoDsl(UseCaseFactory<IIpInfoDriver> useCaseFactory)
    {
        _useCaseFactory = useCaseFactory;
    }

    public GoToIpInfo GoToIpInfo() => _useCaseFactory.Create<GoToIpInfo>();
}