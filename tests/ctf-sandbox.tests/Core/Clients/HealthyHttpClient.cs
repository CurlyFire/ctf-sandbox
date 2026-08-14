using System.Net.Sockets;

namespace ctf_sandbox.tests.Core.Clients;

public abstract class HealthyHttpClient
{
    protected HttpClient HttpClient {get;}

    public HealthyHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<bool> IsHealthy()
    {
        var uri = HttpClient.BaseAddress;
        var port = uri.Port == -1 ? (uri.Scheme == "https" ? 443 : 80) : uri.Port;

        using var client = new TcpClient();
        var connectTask = client.ConnectAsync(uri.Host, port);
        // Use a reasonable timeout
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

        // Wait for either connection or timeout
        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

        return completedTask == connectTask && client.Connected;
    }
}