using ctf_sandbox.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ctf_sandbox.Health;

public class SqliteHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public SqliteHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("Can't connect to SQLite database.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Exception in DB health check", ex);
        }
    }
}
