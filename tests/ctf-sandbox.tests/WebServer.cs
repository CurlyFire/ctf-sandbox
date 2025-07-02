using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests;

public class WebServer : IDisposable
{
    private IHost? _host;
    public string Url { get; private set; }

    public WebServer()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.tests.json", optional: false)
        .AddJsonFile("appsettings.tests.dev.json", optional: true)
        .AddEnvironmentVariables();
        var config = configBuilder.Build();
        var url = config.GetValue<string>("WebServer:Url");

        if (string.IsNullOrWhiteSpace(url))
        {
            Url = HostWithinProcess();
        }
        else
        {
            Url = url;
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