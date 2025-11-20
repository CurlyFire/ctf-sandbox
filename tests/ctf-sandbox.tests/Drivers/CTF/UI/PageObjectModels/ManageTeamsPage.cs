using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.CTF.UI.PageObjectModels;

public class ManageTeamsPage
{
    private readonly IPage _page;

    public ManageTeamsPage(IPage page)
    {
        _page = page;
    }

    public async Task<CreateNewTeamPage> GoToCreateNewTeamPage()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Create New Team" }).ClickAsync();
        return new CreateNewTeamPage(_page);
    }

    public async Task<EditTeamPage> GoToEditTeamPage(string teamName)
    {
        // Find the card containing the team name and click the Edit button
        var teamCard = _page.Locator(".card").Filter(new() { HasText = teamName });
        await teamCard.GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        return new EditTeamPage(_page);
    }

    public async Task<bool> IsTeamVisible(string teamName)
    {
        return await _page.GetByText(teamName).IsVisibleAsync();
    }

    public async Task<ctf_sandbox.Areas.CTF.Models.Team?> GetTeam(string teamName)
    {
        // Check if team is visible first
        if (!await IsTeamVisible(teamName))
        {
            return null;
        }

        // Find the card containing the team name
        var teamCard = _page.Locator(".card").Filter(new() { HasText = teamName });
        
        // Extract member count from the data-testid attribute
        var memberCountLocator = teamCard.Locator($"[data-testid='member-count-{teamName}']");
        var memberCountText = await memberCountLocator.TextContentAsync();
        
        if (string.IsNullOrEmpty(memberCountText) || !uint.TryParse(memberCountText, out var memberCount))
        {
            return null;
        }

        // Return a minimal Team object with the information we can extract
        return new ctf_sandbox.Areas.CTF.Models.Team
        {
            Name = teamName,
            MemberCount = memberCount
        };
    }

}
