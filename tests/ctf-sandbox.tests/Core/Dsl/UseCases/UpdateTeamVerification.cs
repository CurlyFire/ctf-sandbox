using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class UpdateTeamVerification : ResponseVerification<GetTeamResponse>
{
    public UpdateTeamVerification(GetTeamResponse response, UseCaseContext context) : base(response, context)
    {
    }

    public UpdateTeamVerification HasTeamName(string expectedTeamName)
    {
        Response.Name.ShouldBe(Context.GetParamValue(expectedTeamName));
        return this;
    }

    public UpdateTeamVerification HasMemberCount(uint expectedMemberCount)
    {
        Response.MemberCount.ShouldBe(expectedMemberCount);
        return this;
    }

    public UpdateTeamVerification HasDescription(string updatedDescription)
    {
        Response.Description.ShouldBe(updatedDescription);
        return this;
    }
}