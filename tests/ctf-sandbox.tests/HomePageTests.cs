using ctf_sandbox.tests.Drivers.UI.PageObjectModels;
using ctf_sandbox.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests;

public class HomePageTests : EnvironmentTests
{
    private HomePage? _homePage;

    public HomePageTests(EnvironmentFixture fixture) : base(fixture)
    {
    }

    override public void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddSingleton<HomePageFactory>();
        services.AddScoped(sp =>
        {
            var factory = sp.GetRequiredService<HomePageFactory>();
            return factory.CreateHomePage();
        });
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