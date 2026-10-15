using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using ctf_sandbox.tests.Core.Dsl.External.IpInfo.UseCases;
using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.External.IpInfo;

public class IpInfoDsl
{
    private readonly UseCaseFactory<IIpInfoDriver> _useCaseFactory;
    public IpInfoDsl(UseCaseFactory<IIpInfoDriver> useCaseFactory)
    {
        _useCaseFactory = useCaseFactory;
    }

    public GoToIpInfo GoToIpInfo() => _useCaseFactory.Create<GoToIpInfo>();
}