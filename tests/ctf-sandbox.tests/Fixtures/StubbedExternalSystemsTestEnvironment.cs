using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using CtfSandbox.Tests.Core.Drivers.ExternalSystems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddSingleton<IEmailsDriver, APIEmailsRealDriver>();
        //services.AddHttpClient<MailpitRealClient>(ConfigureEmailsHttpClient);
        services.AddSingleton<IBannedWordsDriver, APIBannedWordsRealDriver>();
        //services.AddHttpClient<BannedWordsRealClient>(ConfigureBannedWordsHttpClient);
        services.AddSingleton<IIpInfoDriver, IPInfoRealDriver>();
        //services.AddHttpClient<IpInfoRealClient>(ConfigureIpInfoHttpClient);        
    }
}