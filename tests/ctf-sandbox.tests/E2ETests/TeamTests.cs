using System.Net;
using System.Net.Http.Json;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;
using Microsoft.AspNetCore.Mvc;

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
    [Fact]
    public async Task API_ShouldFailToCreateTeamWithNonIntegerMemberCount()
    {
        var client = _fixture.InteractWithCTFThroughHttpClient();
        var result = await client.PostAsJsonAsync("auth", new LoginRequest
        {
            Username = _fixture.Configuration.WebServerCredentials.Username,
            Password = _fixture.Configuration.WebServerCredentials.Password
        });
        result.EnsureSuccessStatusCode();
        var jwt = await result.Content.ReadAsStringAsync();
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

        result = await client.PostAsJsonAsync("teams", new
        {
            Name = "TeamWithInvalidMemberCount",
            Description = "This team has an invalid member count",
            MemberCount = "five" // Invalid non-integer value
        });

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();
        var errors = problemDetails.Extensions["errors"].ToString();
        Assert.Contains("The JSON value could not be converted to System.UInt32", errors);
    }
}
