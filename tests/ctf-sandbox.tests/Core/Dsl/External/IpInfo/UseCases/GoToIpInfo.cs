using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
namespace ctf_sandbox.tests.Core.Dsl.External.IpInfo.UseCases;

public class GoToIpInfo : IpInfoUseCase<VoidValue, VoidVerification>
{
    public GoToIpInfo(IIpInfoDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public override async Task<IpInfoUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.GoToIpInfo();
        return new IpInfoUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}