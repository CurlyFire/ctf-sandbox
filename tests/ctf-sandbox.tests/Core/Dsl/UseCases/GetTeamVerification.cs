using ctf_sandbox.Areas.CTF.Models;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetTeamVerification : ResponseVerification<Team>
{
    public GetTeamVerification(Team response, UseCaseContext context) : base(response, context)
    {
    }

    public GetTeamVerification HasTeamName(string expectedTeamName)
    {
        Response.Name.ShouldBe(expectedTeamName);
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
