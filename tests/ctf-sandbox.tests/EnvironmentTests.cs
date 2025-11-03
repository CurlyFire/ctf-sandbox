using ctf_sandbox.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests;

public abstract class EnvironmentTests : IDisposable
{
    private IServiceScope _scope;
    private bool disposedValue;
    protected EnvironmentFixture EnvironmentFixture { get; }
    public EnvironmentTests(EnvironmentFixture fixture)
    {
        EnvironmentFixture = fixture;
        var services = new ServiceCollection();
        ConfigureServicesInternal(services);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        ConfigureInternal(_scope.ServiceProvider);
    }

    private void ConfigureServicesInternal(IServiceCollection services)
    {
        services.AddSingleton(EnvironmentFixture.Configuration);
        ConfigureServices(services);
    }

    private void ConfigureInternal(IServiceProvider serviceProvider)
    {
        Configure(serviceProvider);
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {        
    }

    public virtual void Configure(IServiceProvider serviceProvider)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _scope.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}