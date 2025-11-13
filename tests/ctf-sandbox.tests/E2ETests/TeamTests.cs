using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class TeamTests
{
    private readonly RealExternalSystemsCTFFixture _fixture;

    public TeamTests(RealExternalSystemsCTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToCreateTeam(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();
        var randomTeamName = $"team_{Guid.NewGuid()}";

        await ctf.CreateTeam(randomTeamName);

        await ctf.ConfirmTeamIsAvailable(randomTeamName);
    }
}
