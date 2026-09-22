using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class UseCaseFactory<TDriver> where TDriver : notnull
{
    private readonly TDriver _driver;
    private readonly IServiceProvider _serviceProvider;

    public UseCaseFactory(TDriver driver, IServiceProvider serviceProvider)
    {
        _driver = driver;
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : notnull
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, _driver);
    }
}