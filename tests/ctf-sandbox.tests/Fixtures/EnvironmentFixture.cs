using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Fixtures;

public abstract class EnvironmentFixture : IDisposable
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
        var externalWebServerUrl = config.GetValue<string>("ExternalWebServerUrl");
        IConfiguration configuration;
        string webServerUrl;
        if (string.IsNullOrWhiteSpace(externalWebServerUrl))
        {
            var hostBuilder = Program.CreateHostBuilder(Array.Empty<string>());
            hostBuilder.UseEnvironment(Environments.Development);
            hostBuilder.ConfigureAppConfiguration(ConfigureAppConfiguration);
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
        else
        {
            configBuilder = new ConfigurationBuilder();
            Program.ConfigureAppConfiguration(configBuilder);
            var envVariableSources = configBuilder.Sources.Where(s => s is EnvironmentVariablesConfigurationSource);
            foreach (var source in envVariableSources)
            {
                configBuilder.Sources.Remove(source);
            }
            ConfigureAppConfiguration(configBuilder);
            configBuilder.AddEnvironmentVariables();

            webServerUrl = externalWebServerUrl;
            configuration = configBuilder.Build();
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

    protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
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
