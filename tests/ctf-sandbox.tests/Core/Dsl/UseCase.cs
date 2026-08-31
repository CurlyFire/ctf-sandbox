namespace ctf_sandbox.tests.Core.Dsl;

public abstract class UseCase<TDriver, TSuccessResponse, TFailureResponse, TSuccessVerification, TFailureVerification>
{
    protected readonly TDriver _driver;
    protected readonly UseCaseContext _context;

    protected UseCase(TDriver driver, UseCaseContext context)
    {
        _driver = driver;
        _context = context;
    }

    public abstract Task<UseCaseResult<TSuccessResponse, TFailureResponse, TSuccessVerification, TFailureVerification>> Execute();
}
