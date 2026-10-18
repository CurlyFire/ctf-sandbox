using WireMock.Client;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class BannedWordsStubbedClient : BannedWordsRealClient
{
    private readonly IWireMockAdminApi _adminApi;

    public BannedWordsStubbedClient(HttpClient httpClient, IWireMockAdminApi adminApi) : base(httpClient)
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
                : Result.Failure<SystemError>(SystemError.Of($"BannedWords stub is not healthy: {health}"));
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemError>(SystemError.Of($"BannedWords stub health check failed: {ex.Message}"));
        }
    }
}
