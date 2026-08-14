using System.Net;
using ctf_sandbox.tests.Core.Clients.API.Endpoints;
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
        uint memberCount = 5;

        var error = await ctf.CreateTeam(randomTeamName, memberCount);

        Assert.Null(error);
        // Cannot verify creation date because we do not control server time
        await ctf.ConfirmTeamIsAvailable(randomTeamName, memberCount);
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
        uint originalMemberCount = 3;
        uint updatedMemberCount = 6;

        // Arrange: Create a team
        var createError = await ctf.CreateTeam(originalTeamName, originalMemberCount);
        Assert.Null(createError);
        await ctf.ConfirmTeamIsAvailable(originalTeamName, originalMemberCount);

        // Act: Update the team
        await ctf.UpdateTeam(originalTeamName, updatedTeamName, updatedDescription, updatedMemberCount);

        // Assert: Verify the updated team is available and old name is gone
        await ctf.ConfirmTeamIsAvailable(updatedTeamName, updatedMemberCount);
        await ctf.ConfirmTeamIsNotAvailable(originalTeamName, originalMemberCount);
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
        uint memberCount = 4;

        // Act: Attempt to create team
        var error = await ctf.CreateTeam(tooLongTeamName, memberCount);

        // Assert: Creation should fail with validation error about length
        Assert.Contains("The Name must be between 2 and 100 characters long", error, StringComparison.OrdinalIgnoreCase);
        await ctf.ConfirmTeamIsNotAvailable(tooLongTeamName);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldFailToCreateTeamWithMissingName(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();
        uint memberCount = 4;

        // Act: Attempt to create team with null/empty name
        var error = await ctf.CreateTeam(null, memberCount);

        // Assert: Creation should fail with validation error about required field
        Assert.Contains("Name field is required", error, StringComparison.OrdinalIgnoreCase);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldFailToCreateTeamWithBannedWordInName(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        await ctf.SignIn();
        var bannedWordTeamName = "badword_" + Guid.NewGuid();
        uint memberCount = 4;
        await _fixture.ExternalSystems.InteractWithBannedWords().CreateBannedWord(bannedWordTeamName);

        // Act: Attempt to create team with a banned word in the name
        var error = await ctf.CreateTeam(bannedWordTeamName, memberCount);

        // Assert: Creation should fail with error about banned words
        Assert.Contains("banned words", error, StringComparison.OrdinalIgnoreCase);
        await ctf.ConfirmTeamIsNotAvailable(bannedWordTeamName);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [InlineData("five")]
    [InlineData("5a")]
    [InlineData("a5")]
    [InlineData(" 5 ")]
    public async Task API_ShouldFailToCreateTeamWithNonIntegerMemberCount(string memberCount)
    {
        var client = _fixture.InteractWithCTFThroughAPIClient();
        var jwt = await client.Authentication.Authenticate(_fixture.Configuration!.WebServerCredentials.Username,
            _fixture.Configuration.WebServerCredentials.Password);

        
        try
        {
            await client.Teams.CreateTeam("TeamWithInvalidMemberCount", memberCount, jwt);
        }
        catch (UnsuccessfulHttpResponseException ex)
        {
            Assert.Equal(HttpStatusCode.BadRequest, ex.Response.StatusCode);
            var problemDetails = await ex.Response.GetValidationProblemDetails();
            Assert.NotNull(problemDetails);
            Assert.True(problemDetails!.Errors.TryGetValue($"$.{nameof(memberCount)}", out var errors));
            Assert.NotNull(errors);
            Assert.NotEmpty(errors);
            Assert.Contains("The JSON value could not be converted to System.UInt32", errors[0]);
        }
    }
}
