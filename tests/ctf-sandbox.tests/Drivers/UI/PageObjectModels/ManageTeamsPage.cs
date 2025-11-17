using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.UI.PageObjectModels;

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

}
