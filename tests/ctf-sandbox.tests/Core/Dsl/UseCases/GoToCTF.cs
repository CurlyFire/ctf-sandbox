using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GoToCTF : CTFUseCase<VoidValue, VoidVerification>
{
    public GoToCTF(ICTFDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public override async Task<CTFUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.GoToCTF();
        return new CTFUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}