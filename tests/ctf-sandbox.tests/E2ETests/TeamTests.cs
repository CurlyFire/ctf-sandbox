namespace ctf_sandbox.tests.E2ETests;

public class TeamTests : CTFDslTests
{
    [Fact]
    [Trait("Category", "E2E")]
    public async Task ShouldBeAbleToCreateTeam()
    {
        await CTFDsl.SignInAsAdmin();

        var randomTeamName = $"team_{Guid.NewGuid()}";
        await CTFDsl.CreateTeam(randomTeamName);

        Assert.True(await CTFDsl.IsTeamVisible(randomTeamName));
    }
}
