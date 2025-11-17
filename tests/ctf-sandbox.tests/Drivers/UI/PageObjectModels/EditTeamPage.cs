using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.UI.PageObjectModels;

public class EditTeamPage
{
    private readonly IPage _page;

    public EditTeamPage(IPage page)
    {
        _page = page;
    }

    public async Task UpdateTeam(string newTeamName, string? newDescription = null)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync(newTeamName);
        
        if (newDescription != null)
        {
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Description" }).FillAsync(newDescription);
        }
        
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
    }
}
