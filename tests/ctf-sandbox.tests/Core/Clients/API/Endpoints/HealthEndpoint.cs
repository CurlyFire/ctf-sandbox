using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class HealthEndpoint : Endpoint
{
    public HealthEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }

    public async Task<Result<VoidValue, ValidationProblemDetails>> CheckHealth()
    {
        var healthResponse = await JsonHttpClient.GetAsync("/health");
        return healthResponse;
    }
}