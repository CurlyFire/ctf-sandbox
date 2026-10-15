using ctf_sandbox.tests.Core.Dsl.UseCases;

using System.Runtime.CompilerServices;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

public class ThenIpInfo<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;
    private readonly List<Action<GetIpInfoVerification>> _verifications = [];

    internal ThenIpInfo(ThenStage<TSuccessResponse, TSuccessVerification> stage)
    {
        _stage = stage;
    }

    public ThenIpInfo<TSuccessResponse, TSuccessVerification> HasIp(string expectedIp)
    {
        _verifications.Add(v => v.HasIp(expectedIp));
        return this;
    }

    public ThenIpInfo<TSuccessResponse, TSuccessVerification> HasLocationData()
    {
        _verifications.Add(v => v.HasLocationData());
        return this;
    }

    public TaskAwaiter GetAwaiter() => Execute().GetAwaiter();

    private async Task Execute()
    {
        var result = await _stage.GetExecutionResult();
        if (result.Result.ShouldSucceed() is not GetIpInfoVerification verification)
        {
            throw new InvalidOperationException("IpInfo() can only verify the result of a GetIpInfo operation.");
        }

        foreach (var check in _verifications)
        {
            check(verification);
        }
    }
}
