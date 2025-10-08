using ctf_sandbox.tests.Drivers.API;
using ctf_sandbox.tests.Drivers.UI;
using ctf_sandbox.tests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests.Drivers;

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
            Channel.API => _drivers.TryGetValue(channel, out var existingDriver) ? existingDriver : _drivers[channel] = _serviceProvider.GetRequiredService<APICTFDriver>(),
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
    }
}
