using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl;

public abstract class CTFUseCase<TResponse, TVerification>
    where TVerification : ResponseVerification<TResponse>
{
    protected ICTFDriver Driver {get;}
    protected UseCaseContext Context {get;}

    protected CTFUseCase(ICTFDriver driver, UseCaseContext context)
    {
        Driver = driver;
        Context = context;
    }

    public abstract Task<CTFUseCaseResult<TResponse, TVerification>> Execute();
}