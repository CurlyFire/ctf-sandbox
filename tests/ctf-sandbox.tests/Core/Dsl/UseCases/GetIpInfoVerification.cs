using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetIpInfoVerification : ResponseVerification<GetIpInfoResponse>
{
    public GetIpInfoVerification(GetIpInfoResponse response, UseCaseContext context)
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