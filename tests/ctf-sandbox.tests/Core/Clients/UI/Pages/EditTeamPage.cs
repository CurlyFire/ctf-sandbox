using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class EditTeamPage : ErrorPage
{
    public EditTeamPage(IPage page) : base(page)
    {
    }

    public async Task<Result<ManageTeamsPage?, ValidationProblemDetails>> UpdateTeam(string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" }).FillAsync(newTeamName);
        
        if (newDescription != null)
        {
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Description" }).FillAsync(newDescription);
        }
        
        if (memberCount.HasValue)
        {
            var memberCountInput = Page.Locator("input[name='MemberCount']");
            await memberCountInput.FillAsync(memberCount.Value.ToString());
        }
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save Changes" }).ClickAsync();
        var errors = await GetErrors();
        if (errors != null)
        {
            return Result<ManageTeamsPage?, ValidationProblemDetails>.Failure(errors);
        }
        var manageTeamsPage = new ManageTeamsPage(Page);
        return Result<ManageTeamsPage?, ValidationProblemDetails>.Success(manageTeamsPage);
    }
}
