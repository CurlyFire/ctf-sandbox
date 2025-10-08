using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Fixtures;

public class EnvironmentFixture : IDisposable
{
    private IHost? _host;
    private IHostBuilder _hostBuilder;
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
        if (string.IsNullOrWhiteSpace(environmentType))
        {
            throw new InvalidOperationException("Environment type is not configured.");
        }
        if (environmentType == EnvironmentTypes.Internal)
        {
            _hostBuilder = Program.CreateHostBuilder(Array.Empty<string>());
            _hostBuilder.UseEnvironment(Environments.Development);
            _hostBuilder.ConfigureWebHost(webHostBuilder =>
            {
                // Configure the web host to use a random port
                webHostBuilder.UseUrls("http://127.0.0.1:0");
            });
            _host = _hostBuilder.Build();
            _host.Start();
            var webServerUrl = _host.GetWebServerUrl();
            var configuration = _host.Services.GetRequiredService<IConfiguration>();
        }
        else if (environmentType == EnvironmentTypes.External)
        {
            // WebServerUrl = config.GetValue<string>("WebServer:Url");
            // MailpitUrl = config.GetValue<string>("Mailpit:Url");
            // IpInfoUrl = config.GetValue<string>("IpInfo:Url");
            // DatabaseConnectionString = config.GetValue<string>("Database:ConnectionString");

            // WebServerCredentials = new Credentials(
            //     config.GetValue<string>("WebServer:AdminAccount")!,
            //     config.GetValue<string>("WebServer:AdminPassword")!
            // );

            // MailpitCredentials = new Credentials(
            //     config.GetValue<string>("Mailpit:AdminAccount")!,
            //     config.GetValue<string>("Mailpit:AdminPassword")!
            // );
        }
        else
        {
            throw new InvalidOperationException($"Unknown environment type: {environmentType}");
        }
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
