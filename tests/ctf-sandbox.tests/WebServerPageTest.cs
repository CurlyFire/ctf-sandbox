using ctf_sandbox.tests.PageObjectModels;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace ctf_sandbox.tests;

public class WebServerPageTest : PageTest, IClassFixture<WebServer>
{
    protected WebServer WebServer { get; private set; }

    public WebServerPageTest(WebServer webServer)
    {
        WebServer = webServer;
        InitializeAsync().GetAwaiter().GetResult();
    }


    public override BrowserNewContextOptions ContextOptions()
    {
        var options = new BrowserNewContextOptions()
        {
            BaseURL = WebServer.WebServerUrl
        };
        return options;
    }

    public async Task<HomePage> GoToHomePage()
    {
        await Page.GotoAsync(string.Empty);
        var page = new HomePage(Page);
        return page;
    }
}