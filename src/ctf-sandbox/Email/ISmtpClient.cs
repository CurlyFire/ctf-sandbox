using System.Net.Mail;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ctf_sandbox.Email;

public interface ISmtpClient
{
    Task SendMailAsync(MailMessage message);
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
}
