using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.AcceptanceTests;

[Collection(StubbedExternalSystemsTestCollection.Name)]
public class TeamTests : CTFTests
{
    public TeamTests(StubbedExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "Acceptance")]
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
