using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetTeamVerification : ResponseVerification<GetTeamResponse>
{
    public GetTeamVerification(GetTeamResponse response, UseCaseContext context) : base(response, context)
    {
    }

    public GetTeamVerification HasTeamName(string expectedTeamName)
    {
        Response.Name.ShouldBe(Context.GetParamValue(expectedTeamName));
        return this;
    }

    public GetTeamVerification HasMemberCount(uint expectedMemberCount)
    {
        Response.MemberCount.ShouldBe(expectedMemberCount);
        return this;
    }

    public GetTeamVerification HasDescription(string expectedDescription)
    {
        Response.Description.ShouldBe(expectedDescription);
        return this;
    }
}
