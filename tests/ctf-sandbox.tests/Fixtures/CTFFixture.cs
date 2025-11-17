using ctf_sandbox.tests.Drivers;
using ctf_sandbox.tests.Drivers.API;
using ctf_sandbox.tests.Drivers.UI;
using ctf_sandbox.tests.Drivers.UI.PageObjectModels;
using ctf_sandbox.tests.Dsl;
using ctf_sandbox.tests.Extensions;
using ctf_sandbox.tests.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Fixtures;

public abstract class CTFFixture
{
    private const string CTFHttpClientName = "ctf";
    private IHost? _host;
    private IServiceScope _scope = null!;
    private bool disposedValue;

    public CTFConfiguration? Configuration { get; private set; }

    public CTFFixture()
    {
        InitializeWebServer();
        InitializeServiceProvider();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public HttpClient InteractWithCTFThroughHttpClient()
    {
        return _scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(CTFHttpClientName);
    }

    public HomePage InteractWithCTFThroughHomePage()
    {
        return _scope.ServiceProvider.GetRequiredService<HomePage>();
    }

    public CTF InteractWithCTFThrough(Channel channel)
    {
        ICTFDriver driver = channel switch
        {
            Channel.UI => _scope.ServiceProvider.GetRequiredService<UICTFDriver>(),
            Channel.API => _scope.ServiceProvider.GetRequiredService<APICTFDriver>(),
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
        return new CTF(driver, Configuration!);
    }

    /// <summary>
    /// Override to configure additional services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    /// <summary>
    /// Override to configure from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider to configure.</param>
    protected virtual void Configure(IServiceProvider serviceProvider)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _scope.Dispose();
                if (_host != null)
                {
                    _host.StopAsync().Wait();
                    _host.Dispose();
                }
            }
            disposedValue = true;
        }
    }

    /// <summary>
    /// Override to add additional configuration sources to the CTF web host to override certain settings.
    /// </summary>
    /// <example>
    /// protected override void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    /// {
    ///    configBuilder.AddJsonFile("appsettings.tests.ctf.json", optional: true
    /// }
    /// </example>
    /// <param name="configBuilder">The configuration builder to configure.</param>
    protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
    }

    private void InitializeWebServer()
    {
        var testsConfiguration = GetTestsConfiguration();
        var externalWebServerUrl = testsConfiguration.GetValue<string>("ExternalWebServerUrl");
        string webServerUrl;
        IConfiguration ctfWebHostConfiguration;

        if (string.IsNullOrWhiteSpace(externalWebServerUrl))
        {
            _host = StartCTFWebHostInTestProcess();
            webServerUrl = _host.GetWebServerUrl();
            ctfWebHostConfiguration = _host.Services.GetRequiredService<IConfiguration>();
        }
        else
        {
            ctfWebHostConfiguration = GetExternalCTFWebHostConfiguration();
            webServerUrl = externalWebServerUrl;
        }
        Configuration = CreateConfiguration(webServerUrl, ctfWebHostConfiguration);
    }

    private void InitializeServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureServicesInternal(services);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        Configure(_scope.ServiceProvider);
    }

    private IConfiguration GetTestsConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.tests.json", optional: false)
        .AddJsonFile("appsettings.tests.dev.json", optional: true)
        .AddEnvironmentVariables();
        return configBuilder.Build();
    }

    private IHost StartCTFWebHostInTestProcess()
    {
        var hostBuilder = Program.CreateHostBuilder(Array.Empty<string>());
        hostBuilder.UseEnvironment(Environments.Development);
        // Apply any additional configuration
        hostBuilder.ConfigureAppConfiguration(ConfigureAppConfiguration);
        hostBuilder.ConfigureWebHost(webHostBuilder =>
        {
            // Configure the web host to use a random port
            webHostBuilder.UseUrls("http://127.0.0.1:0");
        });
        var host = hostBuilder.Build();
        host.Start();
        return host;
    }

    private IConfiguration GetExternalCTFWebHostConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        // Extract base configuration from Program
        Program.ConfigureAppConfiguration(configBuilder);
        // Remove existing environment variable sources to avoid duplication
        var envVariableSources = configBuilder.Sources.Where(s => s is EnvironmentVariablesConfigurationSource);
        foreach (var source in envVariableSources)
        {
            configBuilder.Sources.Remove(source);
        }
        // Apply any additional configuration
        ConfigureAppConfiguration(configBuilder);
        // Re-add environment variables at the end to ensure they have the highest priority
        configBuilder.AddEnvironmentVariables();

        var configuration = configBuilder.Build();
        return configuration;
    }

    private CTFConfiguration CreateConfiguration(string webServerUrl, IConfiguration ctfWebHostConfiguration)
    {
        if (string.IsNullOrWhiteSpace(webServerUrl))
        {
            throw new InvalidOperationException("Web server URL is not configured.");
        }
        var configuration = new CTFConfiguration(
            webServerUrl,
            ctfWebHostConfiguration.GetRequiredValue<string>("EmailSettings:MailpitUrl"),
            ctfWebHostConfiguration.GetRequiredValue<string>("IPInfo:BaseUrl"),
            ctfWebHostConfiguration.GetRequiredValue<string>("ConnectionStrings:DefaultConnection"),
            new Credentials(
                ctfWebHostConfiguration.GetRequiredValue<string>("AdminAccount:Email"),
                ctfWebHostConfiguration.GetRequiredValue<string>("AdminAccount:Password")
            ),
            new Credentials(
                ctfWebHostConfiguration.GetValue<string>("EmailSettings:Username") ?? string.Empty,
                ctfWebHostConfiguration.GetValue<string>("EmailSettings:Password") ?? string.Empty
            ));
        return configuration;
    }

    private void ConfigureServicesInternal(IServiceCollection services)
    {
        services.AddSingleton(Configuration!);
        services.AddHttpClient(CTFHttpClientName, ConfigureCTFHttpClient);
        services.AddSingleton<HomePageFactory>();
        services.AddTransient(sp =>
        {
            var factory = sp.GetRequiredService<HomePageFactory>();
            return factory.CreateHomePage();
        });
        services.AddTransient<UICTFDriver>();
        services.AddHttpClient<APICTFDriver>(ConfigureCTFHttpClient);
        services.AddHttpClient<APIEmailsDriver>(ConfigureEmailsHttpClient);
        ConfigureServices(services);
    }

    private void ConfigureCTFHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.WebServerUrl + "/api/");
    }

    private void ConfigureEmailsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.MailpitUrl + "/api/v1/");
    }
}