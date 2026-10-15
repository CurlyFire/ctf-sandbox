using ctf_sandbox.tests.Core.Clients.ExternalSystems;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using WireMock.Client;

namespace ctf_sandbox.tests.Fixtures;

public class StubbedExternalSystemsTestEnvironment : TestEnvironment
{
    override protected void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddJsonFile("appsettings.web.stubbed.json", optional: false)
            .AddJsonFile("appsettings.web.stubbed.dev.json", optional: true);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<IClockDriver, ClockStubbedDriver>(sp =>
        {
            var api = RestClient.For<IWireMockAdminApi>();
            return ActivatorUtilities.CreateInstance<ClockStubbedDriver>(sp, api);
        });
        services.AddHttpClient<ClockStubbedClient>(ConfigureClockStubbedHttpClient);
        services.AddSingleton<IEmailsDriver, APIEmailsRealDriver>();
        //services.AddHttpClient<MailpitRealClient>(ConfigureEmailsHttpClient);
        services.AddSingleton<IBannedWordsDriver, APIBannedWordsRealDriver>();
        //services.AddHttpClient<BannedWordsRealClient>(ConfigureBannedWordsHttpClient);
        services.AddSingleton<IIpInfoDriver, IPInfoRealDriver>();
        //services.AddHttpClient<IpInfoRealClient>(ConfigureIpInfoHttpClient);        
    }

    private void ConfigureClockStubbedHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.MailpitUrl + "/api/v1/");
    }
}