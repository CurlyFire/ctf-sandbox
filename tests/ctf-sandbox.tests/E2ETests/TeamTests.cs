using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : IClassFixture<CTFFixture>
{
    private readonly CTFFixture _fixture;

    public TeamTests(CTFFixture fixture)
    {
        _fixture = fixture;
    }


    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToCreateTeam(Channel channel)
    {
        var CTFDsl = _fixture.GetDsl(channel);
        await CTFDsl.SignIn(_fixture.WebServerCredentials.Username, _fixture.WebServerCredentials.Password);

        var randomTeamName = $"team_{Guid.NewGuid()}";
        await CTFDsl.CreateTeam(randomTeamName);

        Assert.True(await CTFDsl.IsTeamVisible(randomTeamName));
    }
}
