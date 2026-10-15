using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class IpInfoEndpoint : Endpoint
{

    public IpInfoEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }
    public async Task<Result<GetIpInfoResponse, ValidationProblemDetails>> GetIpInfo(string ipAddress, string jwt)
    {
        var url = $"ipinfo/{ipAddress}";
        return await JsonHttpClient.GetAsync<GetIpInfoResponse>(url, jwt);
    }
}