using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class ErrorPage
{
    private readonly IPage _page;

    public ErrorPage(IPage page)
    {
        _page = page;
    }

    public async Task<bool> HasErrors()
    {
        throw new NotImplementedException();
    }
}