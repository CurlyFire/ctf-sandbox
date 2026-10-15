using ctf_sandbox.tests.Core.Clients.ExternalSystems;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Fixtures;

public class RealExternalSystemsTestEnvironment : TestEnvironment
{
    override protected void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddJsonFile("appsettings.web.real.json", optional: false)
            .AddJsonFile("appsettings.web.real.dev.json", optional: true);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<IEmailsDriver, APIEmailsRealDriver>();
        services.AddHttpClient<MailpitRealClient>(ConfigureEmailsHttpClient);
        services.AddSingleton<IBannedWordsDriver, APIBannedWordsRealDriver>();
        services.AddHttpClient<BannedWordsRealClient>(ConfigureBannedWordsHttpClient);
        services.AddSingleton<IIpInfoDriver, IPInfoRealDriver>();
        services.AddHttpClient<IpInfoRealClient>(ConfigureIpInfoHttpClient);
    }

    private void ConfigureEmailsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.MailpitUrl + "/api/v1/");
    }

    private void ConfigureBannedWordsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.BannedWordsUrl);
    }

    private void ConfigureIpInfoHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.IpInfoUrl);
    }
}