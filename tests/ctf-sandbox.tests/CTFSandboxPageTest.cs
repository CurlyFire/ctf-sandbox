using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace ctf_sandbox.tests;

public class CTFSandboxPageTest : PageTest, IDisposable
{
    private IHost? _host;
    private string _webServerUrl;

    public CTFSandboxPageTest()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.tests.json", optional: false)
        .AddJsonFile("appsettings.tests.dev.json", optional: true)
        .AddEnvironmentVariables();
        var config = configBuilder.Build();
        var webServerUrl = config.GetValue<string>("WebServer:ExternalUrl");

        if (string.IsNullOrWhiteSpace(webServerUrl))
        {
            var builder = Program.CreateHostBuilder(Array.Empty<string>());
            builder.UseEnvironment(Environments.Development);
            builder.ConfigureWebHost(webHostBuilder =>
            {
                // Configure the web host to use a random port
                webHostBuilder.UseUrls("http://127.0.0.1:0");
            });

            _host = builder.Build();
            _host.RunAsync();
            webServerUrl = _host.GetWebServerUrl();
            if (string.IsNullOrWhiteSpace(webServerUrl))
            {
                throw new InvalidOperationException("Web server URL is not configured.");
            }
        }
        _webServerUrl = webServerUrl;
    }

    public void Dispose()
    {
        _host?.StopAsync().GetAwaiter().GetResult();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        var options = new BrowserNewContextOptions()
        {
            BaseURL = _webServerUrl
        };
        return options;
    }
}