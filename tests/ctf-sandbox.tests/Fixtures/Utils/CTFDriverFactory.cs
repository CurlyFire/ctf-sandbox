using ctf_sandbox.tests.Fixture;
using ctf_sandbox.tests.Fixture.Drivers;
using ctf_sandbox.tests.Fixture.Drivers.UI;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Fixtures.Utils;

public class CTFDriverFactory
{
    private readonly IServiceProvider _serviceProvider;
    private Dictionary<Channel, ICTFDriver> _drivers;
    public CTFDriverFactory(IServiceProvider serviceProvider)
    {
        _drivers = new Dictionary<Channel, ICTFDriver>();
        _serviceProvider = serviceProvider;
    }

    public ICTFDriver Create(Channel channel)
    {
        return channel switch
        {
            Channel.UI => _drivers.TryGetValue(channel, out var existingDriver) ? existingDriver : _drivers[channel] = _serviceProvider.GetRequiredService<UICTFDriver>(),
            Channel.API => throw new NotImplementedException("API driver is not implemented yet."),
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
    }
}
