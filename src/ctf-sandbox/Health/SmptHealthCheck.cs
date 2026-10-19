using ctf_sandbox.Email;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ctf_sandbox.Health;

public class SmtpHealthCheck : IHealthCheck
{
    private readonly ISmtpClient _smtpClient;

    public SmtpHealthCheck(ISmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return _smtpClient.CheckHealthAsync(cancellationToken);
    }
}
