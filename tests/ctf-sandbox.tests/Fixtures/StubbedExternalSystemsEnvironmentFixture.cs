using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.tests.Fixtures;

public class StubbedExternalSystemsEnvironmentFixture : EnvironmentFixture
{
    override protected void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddJsonFile("appsettings.web.stubbed.json", optional: false)
            .AddJsonFile("appsettings.web.stubbed.dev.json", optional: true);
    }
}