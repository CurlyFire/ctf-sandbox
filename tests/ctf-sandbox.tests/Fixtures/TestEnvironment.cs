using ctf_sandbox.tests.Core.Clients.API;
using ctf_sandbox.tests.Core.Clients.API.Endpoints;
using ctf_sandbox.tests.Core.Clients.ExternalSystems;
using ctf_sandbox.tests.Core.Clients.UI;
using ctf_sandbox.tests.Core.Drivers.CTF;
using ctf_sandbox.tests.Core.Drivers.CTF.API;
using ctf_sandbox.tests.Core.Drivers.CTF.UI;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using ctf_sandbox.tests.Core.Dsl;
using ctf_sandbox.tests.Core.Dsl.External.BannedWords;
using ctf_sandbox.tests.Core.Dsl.External.Clock;
using ctf_sandbox.tests.Core.Dsl.External.Emails;
using ctf_sandbox.tests.Core.Dsl.External.IpInfo;
using ctf_sandbox.tests.Core.Dsl.UseCases;
using ctf_sandbox.tests.Extensions;
using ctf_sandbox.tests.Utils;
using ctf_sandbox.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Fixtures;

public abstract class TestEnvironment
{
    private IHost? _host;
    private IServiceScope _scope = null!;
    private readonly System.Collections.Concurrent.ConcurrentBag<IServiceScope> _scenarioScopes = new();
    private IConfiguration? _ctfWebHostConfiguration;
    private bool disposedValue;
    public CTFConfiguration? Configuration { get; private set; }

    public TestEnvironment()
    {
        InitializeWebServer();
        InitializeServiceProvider();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public APIClient InteractWithCTFThroughAPIClient()
    {
        return _scope.ServiceProvider.GetRequiredService<APIClient>();
    }

    public UIClient InteractWithCTFThroughUIClient()
    {
        return _scope.ServiceProvider.GetRequiredService<UIClient>();
    }

    /// <summary>
    /// Interact with the system through the default channel (API).
    /// </summary>
    public UseCaseDsl InteractWithSystem()
    {
        return InteractWithSystemThrough(Channel.API);
    }

    public UseCaseDsl InteractWithSystemThrough(Channel channel)
    {
        // One scope per scenario, so each scenario gets its own UseCaseContext (alias -> unique value map)
        var scenarioScope = _scope.ServiceProvider.CreateScope();
        _scenarioScopes.Add(scenarioScope);
        var sp = scenarioScope.ServiceProvider;
        ICTFDriver driver = channel switch
        {
            Channel.UI => sp.GetRequiredService<UICTFDriver>(),
            Channel.API => sp.GetRequiredService<APICTFDriver>(),
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
        //TODO: Replace all this manual creation with a more robust dependency injection library (autofac)
        var ctfUseCaseFactory = ActivatorUtilities.CreateInstance<UseCaseFactory<ICTFDriver>>(sp, driver);
        var ctf = ActivatorUtilities.CreateInstance<CTFDsl>(sp, ctfUseCaseFactory);

        var ipInfoUseCaseFactory = ActivatorUtilities.CreateInstance<UseCaseFactory<IIpInfoDriver>>(sp, sp.GetRequiredService<IIpInfoDriver>());
        var ipInfo = ActivatorUtilities.CreateInstance<IpInfoDsl>(sp, ipInfoUseCaseFactory);
        var clockUseCaseFactory = ActivatorUtilities.CreateInstance<UseCaseFactory<IClockDriver>>(sp, sp.GetRequiredService<IClockDriver>());
        var clock = ActivatorUtilities.CreateInstance<ClockDsl>(sp, clockUseCaseFactory);
        var email = ActivatorUtilities.CreateInstance<EmailsDsl>(sp);
        var bannedWords = ActivatorUtilities.CreateInstance<BannedWordsDsl>(sp);
        var externalSystems = ActivatorUtilities.CreateInstance<ExternalSystemsDsl>(sp, ipInfo, email, bannedWords, clock);
        return ActivatorUtilities.CreateInstance<UseCaseDsl>(sp, ctf, externalSystems);
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
                foreach (var scenarioScope in _scenarioScopes) scenarioScope.Dispose();
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
        _ctfWebHostConfiguration = ctfWebHostConfiguration;
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

    protected IConfiguration GetTestsConfiguration()
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
            ctfWebHostConfiguration.GetRequiredValue<string>("BannedWords:BaseUrl"),
            ctfWebHostConfiguration.GetRequiredValue<string>("ConnectionStrings:DefaultConnection"),
            new Credentials(
                ctfWebHostConfiguration.GetRequiredValue<string>("AdminAccount:Email"),
                ctfWebHostConfiguration.GetRequiredValue<string>("AdminAccount:Password")
            ),
            new Credentials(
                ctfWebHostConfiguration.GetValue<string>("EmailSettings:Username") ?? string.Empty,
                ctfWebHostConfiguration.GetValue<string>("EmailSettings:Password") ?? string.Empty
            ),
            ctfWebHostConfiguration
                .GetSection(TimeProviderOptions.ConfigurationSection)
                .Get<TimeProviderOptions>() ?? new TimeProviderOptions());
        return configuration;
    }

    private void ConfigureServicesInternal(IServiceCollection services)
    {
        services.AddSingleton(Configuration!);
        services.AddHttpClient().ConfigureHttpClientDefaults(ConfigureCTFHttpClient);
        services.AddTimeProviderBasedOn(_ctfWebHostConfiguration
            ?? throw new InvalidOperationException("CTF web host configuration has not been initialized."));
        services.AddSingleton<ClockRealClient>();
        services.AddSingleton<IClockDriver, ClockRealDriver>();
        services.AddTransient<UIClient>();
        services.AddTransient<UICTFDriver>();
        services.AddTransient<APICTFDriver>();
        services.AddTransient<APIClient>();
        services.AddTransient<AuthenticationEndpoint>();
        services.AddTransient<AccountEndpoint>();
        services.AddTransient<TeamsEndpoint>();
        services.AddTransient<IpInfoEndpoint>();
        services.AddTransient<HealthEndpoint>();
        services.AddSingleton(Playwright.CreateAsync().Result);
        services.AddScoped<GoToCTF>();
        services.AddScoped<SignIn>();
        services.AddScoped<SignInParameters>();
        services.AddScoped<CreateAccount>();
        services.AddScoped<CreateAccountParameters>();
        services.AddScoped<GetIpInfo>();
        services.AddScoped<GetIpInfoParameters>();
        services.AddScoped<CreateTeam>();
        services.AddScoped<CreateTeamParameters>();
        services.AddScoped<UpdateTeam>();
        services.AddScoped<UpdateTeamParameters>();
        services.AddScoped<UseCaseContext>();
        services.AddSingleton<CTFDsl>();
        services.AddSingleton<ExternalSystemsDsl>();
        services.AddSingleton<IpInfoDsl>();
        services.AddSingleton<ClockDsl>();
        services.AddSingleton<ExternalSystemsDsl>();
        services.AddSingleton<EmailsDsl>();
        services.AddSingleton<BannedWordsDsl>();
        ConfigureServices(services);
    }

    private void ConfigureCTFHttpClient(IHttpClientBuilder builder)
    {
        builder.ConfigureHttpClient(httpClient =>
        {
            httpClient.BaseAddress = new Uri(Configuration!.WebServerUrl + "/api/");
        });
    }

    private void ConfigureIpInfoHttpClient(HttpClient client)
    {
        client.BaseAddress = new Uri(Configuration!.IpInfoUrl);
    }
}