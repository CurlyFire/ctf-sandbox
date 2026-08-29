using ctf_sandbox.Models;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class IpInfoEndpoint : Endpoint
{

    public IpInfoEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }
    public async Task<Result<IpInfo, ValidationProblemDetails>> GetIpInfo(string ipAddress, string jwt)
    {
        var url = $"ipinfo/{ipAddress}";
        return await JsonHttpClient.GetAsync<IpInfo>(url, jwt);
    }
}