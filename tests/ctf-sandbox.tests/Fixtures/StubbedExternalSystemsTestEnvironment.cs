using ctf_sandbox.tests.Core.Clients.ExternalSystems;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using ctf_sandbox.tests.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddSingleton<IEmailsDriver, EmailsStubbedDriver>();
        services.AddRestEaseClient<MailpitStubbedClient, IWireMockAdminApi>(ConfigureEmailsHttpClient);
        services.AddSingleton<IBannedWordsDriver, BannedWordsStubbedDriver>();
        services.AddRestEaseClient<BannedWordsStubbedClient, IWireMockAdminApi>(ConfigureBannedWordsHttpClient);
        services.AddSingleton<IIpInfoDriver, IpInfoStubbedDriver>();
        services.AddRestEaseClient<IpInfoStubbedClient, IWireMockAdminApi>(ConfigureIpInfoHttpClient);
        services.AddRestEaseClient<ClockStubbedClient, IWireMockAdminApi>(ConfigureClockHttpClient);
    }

    protected override void Configure(IServiceProvider serviceProvider)
    {
        base.Configure(serviceProvider);
        serviceProvider.GetRequiredService<MailpitStubbedClient>().SetupSendEmail().GetAwaiter().GetResult();
    }

    protected void ConfigureClockHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(Configuration!.TimeProviderOptions.Url!);
    }    
}
