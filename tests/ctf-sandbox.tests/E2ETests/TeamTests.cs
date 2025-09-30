using ctf_sandbox.tests.Fixtures.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : DSLTests
{
    public TeamTests(ServerFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToCreateTeam(Channel channel)
    {
        var CTFDsl = GetDsl(channel);
        var serverConfig = ServerFixture.Configuration;
        await CTFDsl.SignIn(serverConfig.WebServerCredentials.Username, serverConfig.WebServerCredentials.Password);

        var randomTeamName = $"team_{Guid.NewGuid()}";
        await CTFDsl.CreateTeam(randomTeamName);

        Assert.True(await CTFDsl.IsTeamVisible(randomTeamName));
    }
}
