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
        await _fixture.Scenario(channel)
            .When()
                .CreateTeam()
                    .WithTeamName("team")
                    .WithMemberCount(5)
            .Then()
                .ShouldSucceed()
                    .And().Team("team").HasMemberCount(5);
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI, Channel.API)]
    public async Task ShouldBeAbleToUpdateExistingTeam(Channel channel)
    {
        await _fixture.Scenario(channel)
            .Given()
                .Team().WithTeamName("original team").WithMemberCount(3)
            .When()
                .UpdateTeam()
                    .WithOriginalTeamName("original team")
                    .WithTeamName("updated team")
                    .WithDescription("This is an updated team description")
                    .WithMemberCount(6)
            .Then()
                .ShouldSucceed()
                    .And()
                        .Team("updated team")
                            .HasMemberCount(6)
                            .HasDescription("This is an updated team description");
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
        await _fixture.Scenario(channel)
            .Given()
                .BannedWord()
                    .WithWord("badword")
            .When()
                .CreateTeam()
                    .WithTeamName("badword")
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
        await _fixture.Scenario(Channel.API)
            .When()
                .CreateTeam()
                    .WithTeamName("team")
                    .WithMemberCount(memberCount)
            .Then()
                .ShouldFail()
                    .FieldErrorMessageContaining("$.memberCount", "The JSON value could not be converted to System.UInt32");
    }
}
