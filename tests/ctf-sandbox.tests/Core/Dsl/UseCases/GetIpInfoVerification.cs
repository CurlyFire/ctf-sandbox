using ctf_sandbox.Models;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetIpInfoVerification : ResponseVerification<IpInfo>
{
    public GetIpInfoVerification(IpInfo response, UseCaseContext context)
        : base(response, context)
    {
    }

    public GetIpInfoVerification HasIp(string expectedIp)
    {
        Response.Ip.ShouldBe(expectedIp);
        return this;
    }

    public GetIpInfoVerification HasLocationData()
    {
        Response.Hostname.ShouldNotBeNullOrEmpty();
        Response.City.ShouldNotBeNullOrEmpty();
        Response.Region.ShouldNotBeNullOrEmpty();
        Response.Country.ShouldNotBeNullOrEmpty();
        Response.Timezone.ShouldNotBeNullOrEmpty();
        return this;
    }
}