using ctf_sandbox.tests.Core.Dsl;
using CtfSandbox.Tests.Core.Drivers.ExternalSystems;

namespace CtfSandbox.Tests.Core.Dsl.External.IpInfo;

public abstract class IpInfoUseCase<TResponse, TVerification>
    where TVerification : ResponseVerification<TResponse>
{
    protected IIpInfoDriver Driver {get;}
    protected UseCaseContext Context {get;}

    protected IpInfoUseCase(IIpInfoDriver  driver, UseCaseContext context)
    {
        Driver = driver;
        Context = context;
    }

    public abstract Task<IpInfoUseCaseResult<TResponse, TVerification>> Execute();
}