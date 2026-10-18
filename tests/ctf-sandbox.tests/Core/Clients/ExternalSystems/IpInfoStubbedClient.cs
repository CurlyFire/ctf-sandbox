using WireMock.Client;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class IpInfoStubbedClient : IpInfoRealClient
{
    private readonly IWireMockAdminApi _adminApi;

    public IpInfoStubbedClient(HttpClient client, IWireMockAdminApi adminApi) : base(client)
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
                : Result.Failure<SystemError>(SystemError.Of($"IpInfo stub is not healthy: {health}"));
        }
        catch (Exception ex)
        {
            return Result.Failure<SystemError>(SystemError.Of($"IpInfo stub health check failed: {ex.Message}"));
        }
    }
}
