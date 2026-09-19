using ctf_sandbox.tests.Core.Drivers.CTF;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class UseCaseFactory
{
    private readonly ICTFDriver driver;
    private readonly IServiceProvider _serviceProvider;

    public UseCaseFactory(ICTFDriver driver, IServiceProvider serviceProvider)
    {
        this.driver = driver;
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : notnull
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, driver);
    }
}