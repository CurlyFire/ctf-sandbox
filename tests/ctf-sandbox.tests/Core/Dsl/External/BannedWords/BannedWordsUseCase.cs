using ctf_sandbox.tests.Core.Dsl;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.BannedWords;

public abstract class BannedWordsUseCase<TResponse, TVerification>
    where TVerification : ResponseVerification<TResponse>
{
    protected IBannedWordsDriver Driver { get; }
    protected UseCaseContext Context { get; }

    protected BannedWordsUseCase(IBannedWordsDriver driver, UseCaseContext context)
    {
        Driver = driver;
        Context = context;
    }

    public abstract Task<BannedWordsUseCaseResult<TResponse, TVerification>> Execute();
}