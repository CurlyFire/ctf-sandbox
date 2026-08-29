namespace ctf_sandbox.tests.Core.Dsl;

public class CTFUseCaseResult<TSuccessResponse, TSuccessVerification>
    : UseCaseResult<TSuccessResponse, SystemError, TSuccessVerification, SystemErrorFailureVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    public CTFUseCaseResult(
        Result<TSuccessResponse, SystemError> result,
        UseCaseContext context,
        Func<TSuccessResponse, UseCaseContext, TSuccessVerification> verificationFactory)
        : base(result, context, verificationFactory, (error, ctx) => new SystemErrorFailureVerification(error, ctx))
    {
    }
}



