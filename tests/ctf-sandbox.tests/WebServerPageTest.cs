using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace ctf_sandbox.tests;

public class WebServerPageTest : PageTest, IClassFixture<WebServer>
{
    private readonly WebServer _webServer;

    public WebServerPageTest(WebServer webServer)
    {
        _webServer = webServer;
    }


    public override BrowserNewContextOptions ContextOptions()
    {
        var options = new BrowserNewContextOptions()
        {
            BaseURL = _webServer.Url
        };
        return options;
    }
}