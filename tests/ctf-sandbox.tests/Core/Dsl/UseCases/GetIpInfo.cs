using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetIpInfo : CTFUseCase<IpInfo, GetIpInfoVerification>
{
    private readonly GetIpInfoParameters _parameters;

    public GetIpInfo(ICTFDriver driver, UseCaseContext context, GetIpInfoParameters parameters)
        : base(driver, context)
    {
        _parameters = parameters;
    }

    public GetIpInfo With(Action<GetIpInfoParameters> configure)
    {
        configure(_parameters);
        return this;
    }

    public override async Task<CTFUseCaseResult<IpInfo, GetIpInfoVerification>> Execute()
    {
        var result = await Driver.GetIpInfo(_parameters.IpAddress);
        return new CTFUseCaseResult<IpInfo, GetIpInfoVerification>(
            result,
            Context,
            (response, ctx) => new GetIpInfoVerification(response, ctx));
    }
}

public record GetIpInfoParameters
{
    public string IpAddress { get; set; } = string.Empty;
}