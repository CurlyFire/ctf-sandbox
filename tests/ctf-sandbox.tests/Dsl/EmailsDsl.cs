using ctf_sandbox.tests.PageObjectModels;

namespace ctf_sandbox.tests.Dsl;

public class EmailsDsl
{
    private readonly EmailsPage _page;

    public EmailsDsl(EmailsPage page)
    {
        _page = page;
    }

    public async Task<bool> IsInboxAvailable()
    {
        return await _page.IsInboxVisible();
    }
}