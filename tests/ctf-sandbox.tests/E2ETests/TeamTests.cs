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

        var error = await ctf.CreateTeam(randomTeamName);

        Assert.Null(error);
        await ctf.ConfirmTeamIsAvailable(randomTeamName);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToUpdateExistingTeam(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();
        var originalTeamName = $"team_{Guid.NewGuid()}";
        var updatedTeamName = $"updated_{Guid.NewGuid()}";
        var updatedDescription = "This is an updated team description";

        // Arrange: Create a team
        var createError = await ctf.CreateTeam(originalTeamName);
        Assert.Null(createError);
        await ctf.ConfirmTeamIsAvailable(originalTeamName);

        // Act: Update the team
        await ctf.UpdateTeam(originalTeamName, updatedTeamName, updatedDescription);

        // Assert: Verify the updated team is available and old name is gone
        await ctf.ConfirmTeamIsAvailable(updatedTeamName);
        await ctf.ConfirmTeamIsNotAvailable(originalTeamName);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldFailToCreateTeamWithNameTooLong(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();
        // Create a team name with 101 characters (exceeds max of 100)
        var tooLongTeamName = new string('A', 101);

        // Act: Attempt to create team
        var error = await ctf.CreateTeam(tooLongTeamName);

        // Assert: Creation should fail with validation error about length
        Assert.Contains("The Name must be between 2 and 100 characters long", error, StringComparison.OrdinalIgnoreCase);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldFailToCreateTeamWithMissingName(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();

        // Act: Attempt to create team with null/empty name
        var error = await ctf.CreateTeam(null);

        // Assert: Creation should fail with validation error about required field
        Assert.Contains("Name field is required", error, StringComparison.OrdinalIgnoreCase);
    }
}
