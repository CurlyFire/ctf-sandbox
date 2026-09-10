using ctf_sandbox.Areas.CTF.Models;
using Shouldly;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class UpdateTeamVerification : ResponseVerification<Team>
{
    public UpdateTeamVerification(Team response, UseCaseContext context) : base(response, context)
    {
    }

    public UpdateTeamVerification HasTeamName(string expectedTeamName)
    {
        Response.Name.ShouldBe(expectedTeamName);
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