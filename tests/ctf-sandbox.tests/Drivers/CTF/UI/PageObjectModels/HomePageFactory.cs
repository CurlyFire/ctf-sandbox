using ctf_sandbox.tests.Fixtures;
using Microsoft.Playwright;
using Microsoft.Playwright.TestAdapter;

namespace ctf_sandbox.tests.Drivers.CTF.UI.PageObjectModels;

public class HomePageFactory : IDisposable
{
    private IPlaywright _playwright;
    private IBrowser? _browser;
    private CTFConfiguration _environmentConfiguration;
    private bool disposedValue;

    public HomePageFactory(CTFConfiguration environmentConfiguration)
    {
        _environmentConfiguration = environmentConfiguration;
        _playwright = Playwright.CreateAsync().Result;
    }

    public HomePage CreateHomePage()
    {
        if (_browser == null)
        {
            var browserType = _playwright[PlaywrightSettingsProvider.BrowserName];
            _browser = browserType.LaunchAsync().Result;
        }

        var context = _browser.NewContextAsync().Result;
        if (context != null && context.Browser != null)
        {
            var options = new BrowserNewPageOptions
            {
                BaseURL = _environmentConfiguration.WebServerUrl
            };
            var page = context.Browser.NewPageAsync(options).Result;
            page.GotoAsync(string.Empty).Wait();
            return new HomePage(page);
        }
        else
        {
            throw new InvalidOperationException("Failed to create browser context or browser.");
        }
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
