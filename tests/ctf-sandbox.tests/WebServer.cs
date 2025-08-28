using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests;

public class WebServer : ServerConfiguration, IDisposable
{
    private IHost? _host;

    public WebServer()
    {
        if (string.IsNullOrWhiteSpace(WebServerUrl))
        {
            WebServerUrl = HostWithinProcess();
        }
    }

    private string HostWithinProcess()
    {
        var builder = Program.CreateHostBuilder(Array.Empty<string>());
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureWebHost(webHostBuilder =>
        {
            // Configure the web host to use a random port
            webHostBuilder.UseUrls("http://127.0.0.1:0");
        });

        _host = builder.Build();
        _host.Start();
        var webServerUrl = _host.GetWebServerUrl();
        if (string.IsNullOrWhiteSpace(webServerUrl))
        {
            throw new InvalidOperationException("Web server URL is not configured.");
        }
        return webServerUrl;
    }

    public void Dispose()
    {
        _host?.StopAsync().GetAwaiter().GetResult();
    }
}