using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Fixtures;

public class EnvironmentFixture : IDisposable
{
    private IHost? _host;
    private bool disposedValue;

    public EnvironmentConfiguration Configuration { get; }

    public EnvironmentFixture()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.tests.json", optional: false)
        .AddJsonFile("appsettings.tests.dev.json", optional: true)
        .AddEnvironmentVariables();
        var config = configBuilder.Build();
        var environmentType = config.GetValue<string>("Environment:Type");
        IConfiguration configuration;
        string webServerUrl;
        if (string.IsNullOrWhiteSpace(environmentType))
        {
            throw new InvalidOperationException("Environment type is not configured.");
        }
        if (environmentType == EnvironmentTypes.Internal)
        {
            var hostBuilder = Program.CreateHostBuilder(Array.Empty<string>());
            hostBuilder.UseEnvironment(Environments.Development);
            hostBuilder.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.web.internal.json", optional: false)
                      .AddJsonFile("appsettings.web.internal.dev.json", optional: true);
            });
            hostBuilder.ConfigureWebHost(webHostBuilder =>
            {
                // Configure the web host to use a random port
                webHostBuilder.UseUrls("http://127.0.0.1:0");
            });
            _host = hostBuilder.Build();
            _host.Start();
            webServerUrl = _host.GetWebServerUrl();
            configuration = _host.Services.GetRequiredService<IConfiguration>();


        }
        else if (environmentType == EnvironmentTypes.External)
        {
            configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.web.json", optional: false)
            .AddJsonFile("appsettings.web.dev.json", optional: true)
            .AddJsonFile("appsettings.web.external.json", optional: false)
            .AddJsonFile("appsettings.web.external.dev.json", optional: true)
            .AddEnvironmentVariables();

            configuration = configBuilder.Build();
            webServerUrl = configuration.GetRequiredValue<string>("WebServer:Url");
        }
        else
        {
            throw new InvalidOperationException($"Unknown environment type: {environmentType}");
        }

        if (string.IsNullOrWhiteSpace(webServerUrl))
        {
            throw new InvalidOperationException("Web server URL is not configured.");
        }
        Configuration = new EnvironmentConfiguration(
            webServerUrl,
            configuration.GetRequiredValue<string>("EmailSettings:MailpitUrl"),
            configuration.GetRequiredValue<string>("IPInfo:BaseUrl"),
            configuration.GetRequiredValue<string>("ConnectionStrings:DefaultConnection"),
            new Credentials(
                configuration.GetRequiredValue<string>("AdminAccount:Email"),
                configuration.GetRequiredValue<string>("AdminAccount:Password")
            ),
            new Credentials(
                configuration.GetValue<string>("EmailSettings:Username") ?? string.Empty,
                configuration.GetValue<string>("EmailSettings:Password") ?? string.Empty
            ));            
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_host != null)
                {
                    _host.StopAsync().GetAwaiter().GetResult();
                    _host.Dispose();
                }
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
