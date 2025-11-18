using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.CTF.UI.PageObjectModels;

public class CreateNewTeamPage
{
    private readonly IPage _page;

    public CreateNewTeamPage(IPage page)
    {
        _page = page;
    }

    public async Task<string?> CreateTeam(string? teamName)
    {
        var nameInput = _page.GetByRole(AriaRole.Textbox, new() { Name = "Name" });
        
        if (teamName != null)
        {
            await nameInput.FillAsync(teamName);
            
            // Check if input was truncated by maxlength attribute
            var actualValue = await nameInput.InputValueAsync();
            if (actualValue.Length < teamName.Length)
            {
                return $"The Name must be between 2 and 100 characters long.";
            }
        }
        
        // Trigger validation by blurring the field
        await nameInput.BlurAsync();
        
        // Check for client-side validation errors before clicking
        var clientSideErrors = await _page.Locator(".text-danger").AllAsync();
        var visibleClientErrors = new List<string>();
        
        foreach (var element in clientSideErrors)
        {
            if (await element.IsVisibleAsync())
            {
                var text = await element.TextContentAsync();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    visibleClientErrors.Add(text);
                }
            }
        }
        
        // If client-side validation caught errors, return them without submitting
        if (visibleClientErrors.Any())
        {
            return string.Join("; ", visibleClientErrors);
        }
        
        await _page.GetByRole(AriaRole.Button, new() { Name = "Create" }).ClickAsync();
        
        // Wait a moment for navigation or server-side validation to occur
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Check if we're still on the create page (which means server-side validation failed)
        var isStillOnCreatePage = await _page.GetByRole(AriaRole.Button, new() { Name = "Create" }).IsVisibleAsync();
        
        if (isStillOnCreatePage)
        {
            // Try to get server-side validation error messages
            var errorElements = _page.Locator(".text-danger").AllAsync();
            var errors = await errorElements;
            var visibleErrors = new List<string>();
            
            foreach (var element in errors)
            {
                if (await element.IsVisibleAsync())
                {
                    var text = await element.TextContentAsync();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        visibleErrors.Add(text);
                    }
                }
            }
            
            if (visibleErrors.Any())
            {
                return string.Join("; ", visibleErrors);
            }
            return "Validation failed";
        }
        
        return null; // Success, no error
    }
}
