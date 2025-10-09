using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.tests.Fixtures;

public class RealExternalSystemsEnvironmentFixture : EnvironmentFixture
{
    override protected void ConfigureAppConfiguration(IConfigurationBuilder configBuilder)
    {
        configBuilder.AddJsonFile("appsettings.web.real.json", optional: false)
            .AddJsonFile("appsettings.web.real.dev.json", optional: true);
    }
}