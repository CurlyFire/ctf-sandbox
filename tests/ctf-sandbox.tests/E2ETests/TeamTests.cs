using ctf_sandbox.tests.Core.Dsl.Scenario;
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
        var teamName = $"team_{Guid.NewGuid()}";
        const uint memberCount = 5;

        await _fixture.Scenario(channel)
            .When()
                .CreateTeam()
                    .WithTeamName(teamName)
                    .WithMemberCount(memberCount)
            .Then()
                .ShouldSucceed()
                    .Satisfies(v => v
                        .HasMemberCount(memberCount)
                        .HasTeamName(teamName));
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToUpdateExistingTeam(Channel channel)
    {
        var originalTeamName = $"team_{Guid.NewGuid()}";
        var updatedTeamName = $"updated_{Guid.NewGuid()}";
        const string updatedDescription = "This is an updated team description";
        const uint originalMemberCount = 3;
        const uint updatedMemberCount = 6;

        await _fixture.Scenario(channel)
            .Given()
                .TeamCreated(originalTeamName, originalMemberCount)
            .When()
                .UpdateTeam()
                    .WithOriginalTeamName(originalTeamName)
                    .WithTeamName(updatedTeamName)
                    .WithDescription(updatedDescription)
                    .WithMemberCount(updatedMemberCount)
            .Then()
                .ShouldSucceed()
                    .Satisfies(v => v
                        .HasTeamName(updatedTeamName)
                        .HasMemberCount(updatedMemberCount)
                        .HasDescription(updatedDescription));
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.API)]
    public async Task ShouldFailToCreateTeamWithNameTooLong(Channel channel)
    {
        await _fixture.Scenario(channel)
            .When()
            .CreateTeam()
                .WithTeamName(new string('A', 101))
                .WithMemberCount(4)
            .Then()
                .ShouldFail()
                    .FieldErrorMessage("Name", "The Name must be between 2 and 100 characters long.");
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldFailToCreateTeamWithMissingName(Channel channel)
    {
        await _fixture.Scenario(channel)
            .When()
                .CreateTeam()
                    .WithTeamName(string.Empty)
            .Then()
                .ShouldFail()
                    .FieldErrorMessage("Name", "The Name field is required.");
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldFailToCreateTeamWithBannedWordInName(Channel channel)
    {
        var teamName = $"badword_{Guid.NewGuid()}";

        await _fixture.Scenario(channel)
            .Given()
                .BannedWord()
                    .WithWord(teamName)
            .When()
                .CreateTeam()
                    .WithTeamName(teamName)
                    .WithMemberCount(4)
            .Then()
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
        var authenticateResult = await client.Authentication.Authenticate(
            _fixture.Configuration!.WebServerCredentials.Username,
            _fixture.Configuration.WebServerCredentials.Password);
        authenticateResult.ShouldBeSuccess();

        var createTeamResult = await client.Teams.CreateTeam(
            "TeamWithInvalidMemberCount",
            memberCount,
            authenticateResult.Value);

        var problemDetails = createTeamResult.Error;
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails!.Errors.TryGetValue($"$.{nameof(memberCount)}", out var errors));
        Assert.NotNull(errors);
        Assert.NotEmpty(errors);
        Assert.Contains("The JSON value could not be converted to System.UInt32", errors[0]);
    }
}
