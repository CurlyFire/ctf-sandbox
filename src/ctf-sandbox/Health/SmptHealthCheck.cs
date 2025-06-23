using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ctf_sandbox.Health;

public class SmtpHealthCheck : IHealthCheck
{
    private readonly IConfiguration _config;

    public SmtpHealthCheck(IConfiguration config)
    {
        _config = config;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var host = _config["EmailSettings:SmtpServer"];
        var portStr = _config["EmailSettings:SmtpPort"];

        if (!int.TryParse(portStr, out var port))
        {
            return HealthCheckResult.Unhealthy("SMTP port config is invalid.");
        }

        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host, port);
            var timeout = Task.Delay(2000, cancellationToken);

            var completed = await Task.WhenAny(connectTask, timeout);
            if (completed == timeout || !client.Connected)
                return HealthCheckResult.Unhealthy("SMTP server not reachable");

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SMTP connection failed", ex);
        }
    }
}
