using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ctf_sandbox.tests.Extensions;

public static class HostExtensions
{
    public static string GetWebServerUrl(this IHost host)
    {
        var server = host.Services.GetService<IServer>();
        if (server == null)
        {
            throw new InvalidOperationException("Web server is not configured.");
        }

        var addresses = server.Features.Get<IServerAddressesFeature>();
        if (addresses == null || addresses.Addresses.Count == 0)
        {
            throw new InvalidOperationException("Web server addresses are not configured.");
        }
        // Return the first address as the web server URL
        return addresses.Addresses.FirstOrDefault() ?? throw new InvalidOperationException("No web server address found.");

    }

}
