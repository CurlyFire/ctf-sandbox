using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Core.Dsl;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(CTFTestCollection.Name)]
public class TeamTests
{
    private readonly TestEnvironment _testEnvironment;

    public TeamTests(TestEnvironments testEnvironments)
    {
        _testEnvironment = testEnvironments.For(ExternalSystems.Real);
    }

    [Trait("Category", "E2E")]
    [Theory, CombinatorialData]
    public async Task ShouldBeAbleToCreateTeam(Channel channel)
    {
        await _testEnvironment.Scenario(channel)
            .When()
                .CreateTeam()
                    .WithTeamName("team")
                    .WithMemberCount(5)
            .Then()
                .ShouldSucceed()
                    .And().Team("team").HasMemberCount(5);
    }

    [Trait("Category", "E2E")]
    [Theory, CombinatorialData]
    public async Task ShouldBeAbleToUpdateExistingTeam(Channel channel)
    {
        await _testEnvironment.Scenario(channel)
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
    [Theory, CombinatorialData]
    public async Task ShouldFailToCreateTeamWithNameTooLong([CombinatorialValues(Channel.API)] Channel channel)
    {
        await _testEnvironment.Scenario(channel)
            .When()
            .CreateTeam()
                .WithTeamName(new string('A', 101))
                .WithMemberCount(4)
            .Then()
                .ShouldFail()
                    .FieldErrorMessage("Name", "The Name must be between 2 and 100 characters long.");
    }

    [Trait("Category", "E2E")]
    [Theory, CombinatorialData]
    public async Task ShouldFailToCreateTeamWithMissingName(Channel channel)
    {
        await _testEnvironment.Scenario(channel)
            .When()
                .CreateTeam()
                    .WithTeamName(string.Empty)
            .Then()
                .ShouldFail()
                    .FieldErrorMessage("Name", "The Name field is required.");
    }

    [Trait("Category", "E2E")]
    [Theory, CombinatorialData]
    public async Task ShouldFailToCreateTeamWithBannedWordInName([CombinatorialValues(Channel.UI)] Channel channel)
    {
        var teamName = $"badword_{Guid.NewGuid()}";

        await _testEnvironment.Scenario(channel)
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
        await _testEnvironment.Scenario(Channel.API)
            .When()
                .CreateTeam()
                    .WithTeamName("team")
                    .WithMemberCount(memberCount)
            .Then()
                .ShouldFail()
                    .FieldErrorMessageContaining("$.memberCount", "The JSON value could not be converted to System.UInt32");
    }
}
