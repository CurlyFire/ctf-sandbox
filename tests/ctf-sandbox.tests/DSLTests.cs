using ctf_sandbox.tests.Drivers;
using ctf_sandbox.tests.Drivers.UI;
using ctf_sandbox.tests.Dsl;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Fixtures.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests;

public class DSLTests : HomePageTests
{
    private CTFDriverFactory? _CTFDriverFactory;

    public DSLTests(ServerFixture fixture) : base(fixture)
    {
    }

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