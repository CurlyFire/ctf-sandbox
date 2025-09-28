using ctf_sandbox.tests.Fixture;
using ctf_sandbox.tests.Fixture.Drivers.UI;
using ctf_sandbox.tests.Fixture.Dsl;
using ctf_sandbox.tests.Fixtures.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Fixtures;

public class DSLFixture : HomePageFixture
{
    private CTFDriverFactory? _CTFDriverFactory;

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<CTFDriverFactory>();
        services.AddSingleton<UICTFDriver>();
    }

    override public void Configure(IServiceProvider serviceProvider)
    {
        base.Configure(serviceProvider);
        _CTFDriverFactory = serviceProvider.GetRequiredService<CTFDriverFactory>();
    }

    public CTFDsl GetDsl(Channel channel)
    {
        var driver = _CTFDriverFactory!.Create(channel);
        return new CTFDsl(driver);
    }
}