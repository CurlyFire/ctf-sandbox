using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class CreateTeamVerification : ResponseVerification<GetTeamResponse>
{
    public CreateTeamVerification(GetTeamResponse response, UseCaseContext context) : base(response, context)
    {
    }

    public CreateTeamVerification HasTeamName(string expectedTeamName)
    {
        Response.Name.ShouldBe(Context.GetParamValue(expectedTeamName));
        return this;
    }

    public CreateTeamVerification HasMemberCount(uint expectedMemberCount)
    {
        Response.MemberCount.ShouldBe(expectedMemberCount);
        return this;
    }
}