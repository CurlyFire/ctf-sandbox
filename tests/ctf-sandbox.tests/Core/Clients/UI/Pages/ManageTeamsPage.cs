using ctf_sandbox.Areas.CTF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class ManageTeamsPage : ErrorPage
{
    public ManageTeamsPage(IPage page) : base(page)
    {
    }

    public async Task<CreateNewTeamPage> GoToCreateNewTeamPage()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create New Team" }).ClickAsync();
        return new CreateNewTeamPage(Page);
    }

    public async Task<EditTeamPage> GoToEditTeamPage(string teamName)
    {
        // Find the card containing the team name and click the Edit button
        var teamCard = Page.Locator(".card").Filter(new() { HasText = teamName });
        await teamCard.GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        return new EditTeamPage(Page);
    }

    public async Task<bool> IsTeamVisible(string teamName)
    {
        return await Page.GetByText(teamName).IsVisibleAsync();
    }

    public async Task<Result<Team?, ValidationProblemDetails>> GetTeam(string teamName)
    {
        // Check if team is visible first
        if (!await IsTeamVisible(teamName))
        {
            return Result<Team?, ValidationProblemDetails>.Failure(new ValidationProblemDetails { Title = $"Team {teamName} not found" });
        }

        // Find the card containing the team name
        var teamCard = Page.Locator(".card").Filter(new() { HasText = teamName });
        
        var teamNameFromPage = await teamCard.Locator(".card-title").TextContentAsync();
        var descriptionLocator = teamCard.Locator($"[data-testid='team-description-{teamName}']");
        var description = await descriptionLocator.IsVisibleAsync()
            ? await descriptionLocator.TextContentAsync()
            : null;

        // Extract member count from the data-testid attribute
        var memberCountLocator = teamCard.Locator($"[data-testid='member-count-{teamName}']");
        var memberCountText = await memberCountLocator.TextContentAsync();
        uint memberCount = 0;
        
        if (string.IsNullOrEmpty(memberCountText) || !uint.TryParse(memberCountText, out memberCount))
        {
            return Result<Team?, ValidationProblemDetails>.Failure(new ValidationProblemDetails { Title = $"Could not read member count for team {teamName}" });
        }

        

        var team = new Team
        {
            Name = teamNameFromPage ?? string.Empty,
            Description = description,
            MemberCount = memberCount
        };
        return Result<Team?, ValidationProblemDetails>.Success(team);
    }
}
