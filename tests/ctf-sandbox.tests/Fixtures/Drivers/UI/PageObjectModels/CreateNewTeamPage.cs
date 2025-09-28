using Microsoft.Playwright;

namespace ctf_sandbox.tests.Fixture.Drivers.UI.PageObjectModels;

public class CreateNewTeamPage
{
    private readonly IPage _page;

    public CreateNewTeamPage(IPage page)
    {
        _page = page;
    }

    public async Task CreateTeam(string teamName)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync(teamName);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
    }
}
