using ctf_sandbox.tests.Core.Dsl;
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
        (await ctf.SignIn().Execute()).ShouldSucceed();
        var randomTeamName = $"team_{Guid.NewGuid()}";
        uint memberCount = 5;

        // Cannot verify creation date because we do not control server time
        (await ctf.CreateTeam().With(t =>
        {
            t.TeamName = randomTeamName;
            t.MemberCount = memberCount;
        }).Execute())
        .ShouldSucceed()
        .HasMemberCount(memberCount)
        .HasTeamName(randomTeamName);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToUpdateExistingTeam(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        (await ctf.SignIn().Execute()).ShouldSucceed();
        var originalTeamName = $"team_{Guid.NewGuid()}";
        var updatedTeamName = $"updated_{Guid.NewGuid()}";
        var updatedDescription = "This is an updated team description";
        uint originalMemberCount = 3;
        uint updatedMemberCount = 6;

        // Arrange: Create a team
        (await ctf.CreateTeam().With(t =>
        {
            t.TeamName = originalTeamName;
            t.MemberCount = originalMemberCount;
        }).Execute())
        .ShouldSucceed();

        (await ctf.UpdateTeam().With(t =>
        {
            t.OriginalTeamName = originalTeamName;
            t.TeamName = updatedTeamName;
            t.Description = updatedDescription;
            t.MemberCount = updatedMemberCount;
        }).Execute())
        .ShouldSucceed()
        .HasTeamName(updatedTeamName)
        .HasMemberCount(updatedMemberCount)
        .HasDescription(updatedDescription);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.API)]
    public async Task ShouldFailToCreateTeamWithNameTooLong(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        (await ctf.SignIn().Execute()).ShouldSucceed();
        // Create a team name with 101 characters (exceeds max of 100)
        var tooLongTeamName = new string('A', 101);
        uint memberCount = 4;

        // Act: Attempt to create team
        (await ctf.CreateTeam().With(t =>
        {
            t.TeamName = tooLongTeamName;
            t.MemberCount = memberCount;
        }).Execute())
        .ShouldFail()
        .FieldErrorMessage("Name", "The Name must be between 2 and 100 characters long.");
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldFailToCreateTeamWithMissingName(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        (await ctf.SignIn().Execute()).ShouldSucceed();
        uint memberCount = 4;

        (await ctf.CreateTeam().With(t =>
        {
            t.TeamName = string.Empty; // Missing name
            t.MemberCount = memberCount;
        }).Execute())
        .ShouldFail()
        .FieldErrorMessage("Name", "The Name field is required.");
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldFailToCreateTeamWithBannedWordInName(Channel channel)
    {
        var ctf = _fixture.InteractWithCTFThrough(channel);
        (await ctf.SignIn().Execute()).ShouldSucceed();
        var bannedWordTeamName = "badword_" + Guid.NewGuid();
        uint memberCount = 4;
        await _fixture.ExternalSystems.InteractWithBannedWords().CreateBannedWord(bannedWordTeamName);

        // Act: Attempt to create team with a banned word in the name
        (await ctf.CreateTeam().With(t =>
        {
            t.TeamName = bannedWordTeamName;
            t.MemberCount = memberCount;
        }).Execute())
        .ShouldFail()
        .ErrorMessage("Team name contains banned words");

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
        var authenticateResult = await client.Authentication.Authenticate(_fixture.Configuration!.WebServerCredentials.Username,
            _fixture.Configuration.WebServerCredentials.Password);
        authenticateResult.ShouldBeSuccess();

        var createTeamResult = await client.Teams.CreateTeam("TeamWithInvalidMemberCount", memberCount, authenticateResult.Value);
        
        var problemDetails = createTeamResult.Error;
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails!.Errors.TryGetValue($"$.{nameof(memberCount)}", out var errors));
        Assert.NotNull(errors);
        Assert.NotEmpty(errors);
        Assert.Contains("The JSON value could not be converted to System.UInt32", errors[0]);
    }
}
