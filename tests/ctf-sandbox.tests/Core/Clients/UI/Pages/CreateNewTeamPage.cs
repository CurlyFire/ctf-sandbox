using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class CreateNewTeamPage : ErrorPage
{
    public CreateNewTeamPage(IPage page) : base(page)
    {
    }

    public async Task<Result<ManageTeamsPage?, ValidationProblemDetails>> CreateTeam(string? teamName, uint memberCount = 4)
    {
        var nameInput = Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" });
        await nameInput.FillAsync(teamName);
        
        // Set member count
        var memberCountInput = Page.Locator("input[name='MemberCount']");
        await memberCountInput.FillAsync(memberCount.ToString());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
        
        // Wait a moment for navigation or server-side validation to occur
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var errors = await GetErrors();
        if (errors != null)
        {
            return Result<ManageTeamsPage?, ValidationProblemDetails>.Failure(errors);
        }
        return Result<ManageTeamsPage?, ValidationProblemDetails>.Success(new ManageTeamsPage(Page));
    }
}
