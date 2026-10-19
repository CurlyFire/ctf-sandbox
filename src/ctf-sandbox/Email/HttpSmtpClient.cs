using System.Net.Mail;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ctf_sandbox.Email;

public class HttpSmtpClient : ISmtpClient
{
    public const string HTTPClientName = "SmtpClient";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<SmtpClientOptions> _options;

    public HttpSmtpClient(IHttpClientFactory httpClientFactory, IOptions<SmtpClientOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    // Uses the Mailpit send API: POST /api/v1/send
    public async Task SendMailAsync(MailMessage message)
    {
        var payload = new
        {
            From = new { Email = message.From!.Address, Name = message.From.DisplayName },
            To = message.To.Select(a => new { Email = a.Address, Name = a.DisplayName }).ToList(),
            Subject = message.Subject,
            HTML = message.IsBodyHtml ? message.Body : null,
            Text = message.IsBodyHtml ? null : message.Body
        };

        using var client = _httpClientFactory.CreateClient(HTTPClientName);
        var url = $"{_options.Value.Url!.TrimEnd('/')}/api/v1/send";
        var response = await client.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient(HTTPClientName);
            client.Timeout = TimeSpan.FromSeconds(2);
            await client.GetAsync(_options.Value.Url, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Email HTTP endpoint not reachable", ex);
        }
    }
}
