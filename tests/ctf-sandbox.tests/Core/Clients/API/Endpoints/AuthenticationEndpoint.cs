using ctf_sandbox.Models;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class AuthenticationEndpoint : Endpoint
{
    public AuthenticationEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }

    public async Task<Result<string, ValidationProblemDetails>> Authenticate(string username, string password)
    {
        var result = await JsonHttpClient.PostAsync<string>("auth", new LoginRequest
        {
            Username = username,
            Password = password
        });
        return result;
    }
}