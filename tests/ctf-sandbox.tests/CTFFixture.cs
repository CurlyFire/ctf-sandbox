using ctf_sandbox.tests.Drivers;
using ctf_sandbox.tests.Dsl;
using ctf_sandbox.tests.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests;

public class CTFFixture : ServerConfiguration, IDisposable
{
    private IHost? _host;
    private Dictionary<Channel, ICTFDriver> _drivers = new();
    private bool disposedValue;

    public CTFFixture()
    {
        if (string.IsNullOrWhiteSpace(WebServerUrl))
        {
            HostWithinProcess();
        }
    }

    private void HostWithinProcess()
    {
        var builder = Program.CreateHostBuilder(Array.Empty<string>());
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureWebHost(webHostBuilder =>
        {
            // Configure the web host to use a random port
            webHostBuilder.UseUrls("http://127.0.0.1:0");
        });
        _host = builder.Build();
        _host.Start();
        var webServerUrl = _host.GetWebServerUrl();
        if (string.IsNullOrWhiteSpace(webServerUrl))
        {
            throw new InvalidOperationException("Web server URL is not configured.");
        }
        WebServerUrl = webServerUrl;
    }

    public CTFDsl GetDsl(Channel channel)
    {
        var driver = CreateDriver(channel);
        return new CTFDsl(driver);
    }

    private ICTFDriver CreateDriver(Channel channel)
    {
        return channel switch
        {
            Channel.UI => _drivers.TryGetValue(channel, out var existingDriver) ? existingDriver : _drivers[channel] = new UICTFDriver(WebServerUrl),
            Channel.API => throw new NotImplementedException("API driver is not implemented yet."),
            _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _host?.StopAsync().GetAwaiter().GetResult();
                foreach (var driver in _drivers.Values)
                {
                    driver.Dispose();
                }
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}