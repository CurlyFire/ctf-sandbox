using ctf_sandbox.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests;

[Collection("Server Tests")]
public abstract class WebServerTests : IDisposable
{
    private IServiceScope _scope;
    private bool disposedValue;
    protected ServerFixture ServerFixture { get; }
    public WebServerTests(ServerFixture fixture)
    {
        ServerFixture = fixture;
        var services = new ServiceCollection();
        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
        Configure(_scope.ServiceProvider);
    }


    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(ServerFixture.Configuration);
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