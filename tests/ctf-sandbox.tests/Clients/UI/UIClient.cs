using ctf_sandbox.tests.Clients.UI.Pages;
using ctf_sandbox.tests.Fixtures;
using Microsoft.Playwright;
using Microsoft.Playwright.TestAdapter;

namespace ctf_sandbox.tests.Clients.UI;

public class UIClient : IDisposable
{
    private IPlaywright _playwright;
    private IBrowser? _browser;
    private HomePage? _homePage;
    private IPage? _currentPage;
    private CTFConfiguration _environmentConfiguration;
    private bool disposedValue;

    public UIClient(CTFConfiguration environmentConfiguration, IPlaywright playwright)
    {
        _environmentConfiguration = environmentConfiguration;
        _playwright = playwright;
    }

    public async Task<HomePage> OpenHomePage()
    {
        if (_browser == null)
        {
            var browserType = _playwright[PlaywrightSettingsProvider.BrowserName];
            _browser = browserType.LaunchAsync().Result;
            var context = _browser.NewContextAsync().Result;
            if (context == null || context.Browser == null)
            {
                throw new InvalidOperationException("Failed to create browser context or browser.");
            }

            var options = new BrowserNewPageOptions
            {
                BaseURL = _environmentConfiguration.WebServerUrl
            };
            _currentPage = context.Browser.NewPageAsync(options).Result;
            _homePage = new HomePage(_currentPage);
        }
        
        if (_homePage == null)
        {
            throw new InvalidOperationException("Failed to initialize home page.");
        }
        await _currentPage!.GotoAsync(string.Empty);
        return _homePage;
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
