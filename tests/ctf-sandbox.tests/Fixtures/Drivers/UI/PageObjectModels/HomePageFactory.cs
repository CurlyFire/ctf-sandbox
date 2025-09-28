using Microsoft.Playwright;
using Microsoft.Playwright.TestAdapter;

namespace ctf_sandbox.tests.Fixture.Drivers.UI.PageObjectModels;

public class HomePageFactory : IDisposable
{
    private IPlaywright _playwright;
    private IServerConfiguration _serverConfiguration;
    private bool disposedValue;

    public HomePageFactory(IServerConfiguration serverConfiguration)
    {
        _serverConfiguration = serverConfiguration;
        _playwright = Playwright.CreateAsync().Result;
    }

    public HomePage CreateHomePage()
    {
        var browserType = _playwright[PlaywrightSettingsProvider.BrowserName];
        var browser = browserType.LaunchAsync().Result;
        var options = new BrowserNewPageOptions
        {
            BaseURL = _serverConfiguration.WebServerUrl
        };
        var page = browser.NewPageAsync(options).Result;
        page.GotoAsync(string.Empty).Wait();
        return new HomePage(page);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _playwright.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}