using ctf_sandbox.tests.Fixtures.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : CTFTests
{
    public TeamTests(ServerFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToCreateTeam(Channel channel)
    {
        var ctf = InteractWithCTFThrough(channel);
        await ctf.SignIn();
        var randomTeamName = $"team_{Guid.NewGuid()}";
        
        await ctf.CreateTeam(randomTeamName);

        await ctf.ConfirmTeamIsAvailable(randomTeamName);
    }
}
