using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.CTF.UI.PageObjectModels;

public class EditTeamPage
{
    private readonly IPage _page;

    public EditTeamPage(IPage page)
    {
        _page = page;
    }

    public async Task UpdateTeam(string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync(newTeamName);
        
        if (newDescription != null)
        {
            await _page.GetByRole(AriaRole.Textbox, new() { Name = "Description" }).FillAsync(newDescription);
        }
        
        if (memberCount.HasValue)
        {
            var memberCountInput = _page.Locator("input[name='MemberCount']");
            await memberCountInput.FillAsync(memberCount.Value.ToString());
        }
        
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
    }
}
