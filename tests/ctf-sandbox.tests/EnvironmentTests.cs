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
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        Configure(_scope.ServiceProvider);
    }


    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(EnvironmentFixture.Configuration);
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