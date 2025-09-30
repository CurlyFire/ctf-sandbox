using ctf_sandbox.tests.Extensions;
using ctf_sandbox.tests.Fixtures.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Fixtures;

public class ServerFixture : IAsyncLifetime
{
    private IHost? _host;
    public ServerConfiguration Configuration { get; private set; }

    public ServerFixture()
    {
        Configuration = new ServerConfiguration();
    }

    public async Task InitializeAsync()
    {
        if (string.IsNullOrWhiteSpace(Configuration.WebServerUrl))
        {
            await HostWithinProcess();
        }
    }

    public async Task DisposeAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }

    private async Task HostWithinProcess()
    {
        var builder = Program.CreateHostBuilder(Array.Empty<string>());
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureWebHost(webHostBuilder =>
        {
            // Configure the web host to use a random port
            webHostBuilder.UseUrls("http://127.0.0.1:0");
        });
        _host = builder.Build();
        await _host.StartAsync();
        var webServerUrl = _host.GetWebServerUrl();
        if (string.IsNullOrWhiteSpace(webServerUrl))
        {
            throw new InvalidOperationException("Web server URL is not configured.");
        }
        Configuration.WebServerUrl = webServerUrl;
    }
}
