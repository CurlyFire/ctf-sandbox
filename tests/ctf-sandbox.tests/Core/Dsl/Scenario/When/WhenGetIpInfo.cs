using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.Scenario.When;

public class WhenGetIpInfo : BaseWhen<IpInfo, GetIpInfoVerification>
{
    private string _ipAddress = string.Empty;

    internal WhenGetIpInfo(UseCaseDsl app, ScenarioDsl scenario, Func<Task> setup)
        : base(app, scenario, setup)
    {
    }

    public WhenGetIpInfo WithIpAddress(string ipAddress)
    {
        _ipAddress = ipAddress;
        return this;
    }

    protected override async Task<ExecutionResult<IpInfo, GetIpInfoVerification>> Execute(UseCaseDsl app)
    {
        var result = await app.CTF.GetIpInfo().With(ipInfo => ipInfo.IpAddress = _ipAddress).Execute();
        return new ExecutionResult<IpInfo, GetIpInfoVerification>(result);
    }
}
