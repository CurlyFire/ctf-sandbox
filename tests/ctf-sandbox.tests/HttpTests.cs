using ctf_sandbox.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests;

public class HttpTests : EnvironmentTests
{
    public HttpTests(EnvironmentFixture fixture) : base(fixture)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.ConfigureHttpClient((serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetRequiredService<EnvironmentConfiguration>();
                client.BaseAddress = new Uri(configuration.WebServerUrl + "/api/");
            });
        });
    }
}
