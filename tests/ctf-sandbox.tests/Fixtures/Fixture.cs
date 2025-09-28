using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Fixtures;

public abstract class Fixture : IDisposable
{
    private bool disposedValue;
    private IServiceScope _scope;

    public Fixture()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        Configure(_scope.ServiceProvider);
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
            _scope.Dispose();
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
