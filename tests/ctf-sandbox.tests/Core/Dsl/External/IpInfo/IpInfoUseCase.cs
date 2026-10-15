using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.IpInfo;

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