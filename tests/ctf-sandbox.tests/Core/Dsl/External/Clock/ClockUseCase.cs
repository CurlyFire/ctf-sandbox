using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Clock;

public abstract class ClockUseCase<TResponse, TVerification>
    where TVerification : ResponseVerification<TResponse>
{
    protected IClockDriver Driver { get; }
    protected UseCaseContext Context { get; }

    protected ClockUseCase(IClockDriver driver, UseCaseContext context)
    {
        Driver = driver;
        Context = context;
    }

    public abstract Task<ClockUseCaseResult<TResponse, TVerification>> Execute();
}
