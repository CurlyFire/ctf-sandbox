using ctf_sandbox.tests.Fixture.Drivers.UI;
using ctf_sandbox.tests.Fixture.Drivers.UI.PageObjectModels;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Fixtures;

public class HomePageFixture : ServerFixture
{
    private HomePage? _homePage;
    override public void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<HomePageFactory>();
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<HomePageFactory>();
            return factory.CreateHomePage();
        });
        services.AddSingleton<UICTFDriver>();
    }

    public override void Configure(IServiceProvider serviceProvider)
    {
        base.Configure(serviceProvider);
        _homePage = serviceProvider.GetRequiredService<HomePage>();
    }

    public HomePage GetHomePage()
    {
        if (_homePage == null)
        {
            throw new InvalidOperationException("HomePage is not initialized. Ensure that the fixture is properly configured.");
        }
        return _homePage;
    }
}
