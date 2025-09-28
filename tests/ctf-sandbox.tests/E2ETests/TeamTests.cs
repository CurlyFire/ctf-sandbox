using ctf_sandbox.tests.Fixture;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : IClassFixture<DSLFixture>
{
    private readonly DSLFixture _fixture;

    public TeamTests(DSLFixture fixture)
    {
        _fixture = fixture;
    }


    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToCreateTeam(Channel channel)
    {
        var CTFDsl = _fixture.GetDsl(channel);
        var serverConfig = _fixture.GetServerConfiguration();
        await CTFDsl.SignIn(serverConfig.WebServerCredentials.Username, serverConfig.WebServerCredentials.Password);

        var randomTeamName = $"team_{Guid.NewGuid()}";
        await CTFDsl.CreateTeam(randomTeamName);

        Assert.True(await CTFDsl.IsTeamVisible(randomTeamName));
    }
}
