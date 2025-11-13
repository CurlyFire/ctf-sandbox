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
    private IHost? _host;
    public CTFConfiguration Configuration { get; private set; }
    private IServiceScope _scope;
    private bool disposedValue;
    private const string CTFHttpClientName = "ctf";
    private const string EmailsHttpClientName = "emails";

    public CTFFixture()
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
        Configuration = new CTFConfiguration(
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
        
        var services = new ServiceCollection();
        ConfigureServicesInternal(services);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        ConfigureInternal(_scope.ServiceProvider);        
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

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    
    private void ConfigureServicesInternal(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        services.AddHttpClient(CTFHttpClientName, ConfigureCTFHttpClient);
        services.AddHttpClient(EmailsHttpClientName, ConfigureEmailsHttpClient);
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

    private void ConfigureInternal(IServiceProvider serviceProvider)
    {
        Configure(serviceProvider);
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {        
    }

    public virtual void Configure(IServiceProvider serviceProvider)
    {
    }
    private void ConfigureCTFHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration.WebServerUrl + "/api/");
    }

    private void ConfigureEmailsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration.MailpitUrl + "/api/v1/");
    }

    protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
    }

    public HttpClient GetCTFHttpClient()
    {
        return _scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(CTFHttpClientName);
    }

    public HttpClient GetEmailsHttpClient()
    {
        return _scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(EmailsHttpClientName);
    }

    public HomePage GetNewHomePage()
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
        return new CTF(driver, Configuration);
    }
}