using ctf_sandbox.tests.Core;
using ctf_sandbox.tests.Core.Dsl;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.BannedWords.UseCases;

public class GoToBannedWords : BannedWordsUseCase<VoidValue, VoidVerification>
{
    public GoToBannedWords(IBannedWordsDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public override async Task<BannedWordsUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.GoToBannedWords();
        return new BannedWordsUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}