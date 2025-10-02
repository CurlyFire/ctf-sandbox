using Microsoft.Extensions.Options;

namespace ctf_sandbox.Time;

public class HttpTimeProvider : TimeProvider
{
    public const string HTTPClientName = "TimeProvider";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TimeProviderOptions> _options;

    public HttpTimeProvider(IHttpClientFactory httpClientFactory, IOptions<TimeProviderOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public override DateTimeOffset GetUtcNow()
    {
        using (var client = _httpClientFactory.CreateClient(HTTPClientName))
        {
            var response = client.GetStringAsync(_options.Value.Url).GetAwaiter().GetResult();
            if (DateTimeOffset.TryParse(response, out var dateTimeOffset))
            {
                return dateTimeOffset;
            }
            throw new InvalidOperationException("Failed to parse time from HTTP response.");
        }
    }
}
