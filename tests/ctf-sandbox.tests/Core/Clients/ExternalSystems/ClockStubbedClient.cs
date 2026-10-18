using WireMock.Client;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class ClockStubbedClient
{
    private readonly HttpClient _httpClient;
    private readonly IWireMockAdminApi _adminApi;

    public ClockStubbedClient(HttpClient httpClient, IWireMockAdminApi adminApi)
    {
        _httpClient = httpClient;
        _adminApi = adminApi;
    }

    public async Task<Result<VoidValue, SystemError>> CheckHealth()
    {
        try
        {
            var health = await _adminApi.GetHealthAsync();
            return string.Equals(health?.Trim(), "Healthy", StringComparison.OrdinalIgnoreCase)
                ? Result.Success<SystemError>()
                : Result.Failure<SystemError>(SystemError.Of($"Clock stub is not healthy: {health}"));
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemError>(SystemError.Of($"Clock stub health check failed: {ex.Message}"));
        }
    }
}
