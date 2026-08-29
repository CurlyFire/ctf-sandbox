using System.Net.Sockets;

namespace ctf_sandbox.tests.Core.Clients;

public abstract class HealthyTcpClient
{
    private readonly Uri _baseAddress;

    public HealthyTcpClient(Uri baseAddress)
    {
        _baseAddress = baseAddress;
    }

    public async Task<bool> IsHealthy()
    {
        var port = _baseAddress.Port == -1 ? (_baseAddress.Scheme == "https" ? 443 : 80) : _baseAddress.Port;

        using var client = new TcpClient();
        var connectTask = client.ConnectAsync(_baseAddress.Host, port);
        // Use a reasonable timeout
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

        // Wait for either connection or timeout
        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

        return completedTask == connectTask && client.Connected;
    }
}