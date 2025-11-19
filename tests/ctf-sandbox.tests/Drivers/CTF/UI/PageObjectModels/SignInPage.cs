using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.CTF.UI.PageObjectModels;

public class SignInPage
{
    private readonly IPage _page;

    public SignInPage(IPage page)
    {
        _page = page;
    }

    public async Task<HomePage> SignIn(string handle, string accessCode)
    {
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Handle" }).FillAsync(handle);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Access Code" }).FillAsync(accessCode);
        await _page.GetByRole(AriaRole.Button, new() { Name = "AUTHENTICATE" }).ClickAsync();
        return new HomePage(_page);
    }

}
