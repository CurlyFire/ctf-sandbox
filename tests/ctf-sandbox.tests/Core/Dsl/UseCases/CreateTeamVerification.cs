using ctf_sandbox.Areas.CTF.Models;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class CreateTeamVerification : ResponseVerification<Team>
{
    public CreateTeamVerification(Team response, UseCaseContext context) : base(response, context)
    {
    }

    public CreateTeamVerification HasTeamName(string expectedTeamName)
    {
        Response.Name.ShouldBe(expectedTeamName);
        return this;
    }

    public CreateTeamVerification HasMemberCount(uint expectedMemberCount)
    {
        Response.MemberCount.ShouldBe(expectedMemberCount);
        return this;
    }
}