using System.Net.Mail;
using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ctf_sandbox.Email;

public class RealSmtpClient : ISmtpClient
{
    private readonly IOptions<SmtpClientOptions> _options;

    public RealSmtpClient(IOptions<SmtpClientOptions> options)
    {
        _options = options;
    }

    public async Task SendMailAsync(MailMessage message)
    {
        using var smtpClient = new SmtpClient(_options.Value.SmtpServer)
        {
            Port = _options.Value.SmtpPort,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = false
        };

        await smtpClient.SendMailAsync(message);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        var host = _options.Value.SmtpServer;
        if (string.IsNullOrWhiteSpace(host))
        {
            return HealthCheckResult.Unhealthy("SMTP server config is invalid.");
        }

        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host, _options.Value.SmtpPort);
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
