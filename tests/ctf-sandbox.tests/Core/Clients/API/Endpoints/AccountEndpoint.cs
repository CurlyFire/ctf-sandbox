using ctf_sandbox.Models;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class AccountEndpoint : Endpoint
{
    public AccountEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }


    public async Task<Result<VoidValue, ValidationProblemDetails>> CreateAccount(string email, string password)
    {
        var result = await JsonHttpClient.PostAsync("account", new RegisterAccountRequest
        {
            Email = email,
            Password = password
        });
        return result;
    }
}