using ctf_sandbox.Models;

namespace ctf_sandbox.tests.Clients.API.Endpoints;

public class IpInfoEndpoint : Endpoint
{
    public IpInfoEndpoint(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress, string jwt)
    {
        var url = $"ipinfo/{ipAddress}";
        return await GetAsyncAndEnsureSuccess<IpInfo>(url, jwt);
    }
}