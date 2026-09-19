using ctf_sandbox.tests.Core.Drivers.CTF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class SignInPage : ErrorPage
{
    public SignInPage(IPage page) : base(page)
    {
    }

    public async Task<Result<HomePage?, ValidationProblemDetails>> SignIn(string? handle, string? accessCode)
    {
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Handle" }).FillAsync(handle ?? string.Empty);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Access Code" }).FillAsync(accessCode ?? string.Empty);
        await Page.GetByRole(AriaRole.Button, new() { Name = "AUTHENTICATE" }).ClickAsync();
        var errors = await GetErrors();
        if (errors != null)
        {
            return Result<HomePage?, ValidationProblemDetails>.Failure(errors);
        }
        return Result<HomePage?, ValidationProblemDetails>.Success(new HomePage(Page));
    }

}
