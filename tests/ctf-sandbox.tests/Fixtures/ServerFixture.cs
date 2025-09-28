using ctf_sandbox.tests.Extensions;
using ctf_sandbox.tests.Fixtures.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Fixtures;

public class ServerFixture : Fixture
{
    private IHost? _host;
    private IServerConfiguration? _serverConfiguration;

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<IServerConfiguration, ServerConfiguration>();
    }

    public override void Configure(IServiceProvider serviceProvider)
    {
        base.Configure(serviceProvider);
        _serverConfiguration = serviceProvider.GetRequiredService<IServerConfiguration>();
        if (string.IsNullOrWhiteSpace(_serverConfiguration.WebServerUrl))
        {
            HostWithinProcess(_serverConfiguration);
        }
    }

    private void HostWithinProcess(IServerConfiguration serverConfiguration)
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
        serverConfiguration.WebServerUrl = webServerUrl;
    }

    public IServerConfiguration GetServerConfiguration()
    {
        return _serverConfiguration!;
    }
}
