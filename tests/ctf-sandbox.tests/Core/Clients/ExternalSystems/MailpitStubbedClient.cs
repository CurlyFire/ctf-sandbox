using System.Text.Json;
using System.Text.RegularExpressions;
using WireMock.Admin.Mappings;
using WireMock.Client;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class MailpitStubbedClient : MailpitRealClient
{
    private readonly IWireMockAdminApi _adminApi;

    public MailpitStubbedClient(HttpClient httpClient, IWireMockAdminApi adminApi) : base(httpClient)
    {
        _adminApi = adminApi;
    }

    public async Task<Result<VoidValue, SystemError>> CheckHealth()
    {
        try
        {
            var health = await _adminApi.GetHealthAsync();
            return string.Equals(health?.Trim(), "Healthy", StringComparison.OrdinalIgnoreCase)
                ? Result.Success<SystemError>()
                : Result.Failure<SystemError>(SystemError.Of($"Mailpit stub is not healthy: {health}"));
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemError>(SystemError.Of($"Mailpit stub health check failed: {ex.Message}"));
        }
    }

    public Task SetupSendEmail() =>
        PostMapping("POST", "/send", new { ID = "stub" });

    public async Task SetupEmailSentTo(string email, string subject, string text)
    {
        var messageId = Guid.NewGuid().ToString();
        await PostMapping("GET", "/search", new
        {
            total = 1,
            count = 1,
            messages = new[] { new { ID = messageId, Subject = subject } }
        }, new ParamModel
        {
            Name = "query",
            Matchers = new[] { new MatcherModel { Name = "ExactMatcher", Pattern = $"to:{email}" } }
        });
        await PostMapping("GET", $"/message/{messageId}", new { ID = messageId, Subject = subject, Text = text });
    }

    // The activation token only exists once the application has sent the email, so we
    // rebuild the registration email from what WireMock recorded.
    public async Task SetupRegistrationEmailSentTo(string email)
    {
        var sent = await FindSentEmail(email)
            ?? throw new InvalidOperationException($"No emails found for {email}");
        await SetupEmailSentTo(email, sent.Subject, HtmlToMailpitText(sent.Html));
    }

    public async Task Reset()
    {
        await _adminApi.ResetMappingsAsync();
        await _adminApi.ResetRequestsAsync();
    }

    private Task PostMapping(string method, string path, object body, params ParamModel[] queryParams) =>
        _adminApi.PostMappingAsync(new MappingModel
        {
            Request = new RequestModel
            {
                Path = ApiPath + path,
                Methods = new[] { method },
                Params = queryParams.Length == 0 ? null : queryParams
            },
            Response = new ResponseModel { StatusCode = 200, BodyAsJson = body }
        });

    private async Task<(string Subject, string Html)?> FindSentEmail(string email)
    {
        var requests = await _adminApi.GetRequestsAsync();
        foreach (var entry in requests.Reverse())
        {
            if (entry.Request?.Path != $"{ApiPath}/send" || string.IsNullOrEmpty(entry.Request.Body))
            {
                continue;
            }

            using var doc = JsonDocument.Parse(entry.Request.Body);
            var root = doc.RootElement;
            var recipients = root.GetProperty("To").EnumerateArray()
                .Select(t => t.GetProperty("Email").GetString());
            if (recipients.Contains(email, StringComparer.OrdinalIgnoreCase))
            {
                var html = root.TryGetProperty("HTML", out var h) ? h.GetString() ?? "" : "";
                return (root.GetProperty("Subject").GetString() ?? "", html);
            }
        }
        return null;
    }

    // Mimics Mailpit's text rendering of links: "label ( url )"
    private static string HtmlToMailpitText(string html)
    {
        var withLinks = Regex.Replace(html, @"<a\s+[^>]*href=['""]([^'""]+)['""][^>]*>(.*?)</a>", "$2 ( $1 )",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return System.Net.WebUtility.HtmlDecode(Regex.Replace(withLinks, "<[^>]+>", ""));
    }
}
