using ctf_sandbox.tests.Core;
using ctf_sandbox.tests.Core.Dsl;

namespace ctf_sandbox.tests.Core.Dsl.External.BannedWords;

public class BannedWordsUseCaseResult<TSuccessResponse, TSuccessVerification>
    : UseCaseResult<TSuccessResponse, SystemError, TSuccessVerification, SystemErrorFailureVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    public BannedWordsUseCaseResult(
        Result<TSuccessResponse, SystemError> result,
        UseCaseContext context,
        Func<TSuccessResponse, UseCaseContext, TSuccessVerification> verificationFactory)
        : base(result, context, verificationFactory, (error, ctx) => new SystemErrorFailureVerification(error, ctx))
    {
    }
}