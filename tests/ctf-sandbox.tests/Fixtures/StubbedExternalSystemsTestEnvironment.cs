using ctf_sandbox.tests.Core.Clients.ExternalSystems;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using ctf_sandbox.tests.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using WireMock.Client;

namespace ctf_sandbox.tests.Fixtures;

public class StubbedExternalSystemsTestEnvironment : RealExternalSystemsTestEnvironment
{
    override protected void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddJsonFile("appsettings.web.stubbed.json", optional: false)
            .AddJsonFile("appsettings.web.stubbed.dev.json", optional: true);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IClockDriver, ClockStubbedDriver>();
        services.AddSingleton<IEmailsDriver, APIEmailsRealDriver>();
        services.AddHttpClient<MailpitRealClient>(ConfigureEmailsHttpClient);
        services.AddSingleton<IBannedWordsDriver, BannedWordsStubbedDriver>();
        services.AddRestEaseClient<BannedWordsStubbedClient, IWireMockAdminApi>(() => Configuration!.BannedWordsUrl);
        services.AddSingleton<IIpInfoDriver, IpInfoStubbedDriver>();
        services.AddRestEaseClient<IpInfoStubbedClient, IWireMockAdminApi>(() => Configuration!.IpInfoUrl);
        services.AddRestEaseClient<ClockStubbedClient, IWireMockAdminApi>(() => Configuration!.TimeProviderOptions.Url!);
    }
}
