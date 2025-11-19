using ctf_sandbox.tests.Drivers.ExternalSystems;
using ctf_sandbox.tests.Dsl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Fixtures;

public class RealExternalSystemsCTFFixture : CTFFixture
{

    private ExternalSystems? _externalSystems;
    public ExternalSystems ExternalSystems
    {
        get
        {
            if (_externalSystems == null)
            {
                throw new InvalidOperationException("ExternalSystems has not been initialized yet.");
            }
            return _externalSystems;
        }
    }

    override protected void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddJsonFile("appsettings.web.real.json", optional: false)
            .AddJsonFile("appsettings.web.real.dev.json", optional: true);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<ExternalSystems>();
        services.AddSingleton<Emails>();
        services.AddHttpClient<IEmailsDriver, APIEmailsDriver>(ConfigureEmailsHttpClient);
        services.AddSingleton<BannedWords>();
        services.AddHttpClient<IBannedWordsDriver, APIBannedWordsDriver>(ConfigureBannedWordsHttpClient);
    }

    protected override void Configure(IServiceProvider serviceProvider)
    {
        base.Configure(serviceProvider);
        _externalSystems = serviceProvider.GetRequiredService<ExternalSystems>();
    }

    private void ConfigureEmailsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.MailpitUrl + "/api/v1/");
    }

    private void ConfigureBannedWordsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.BannedWordsUrl);
    }
}