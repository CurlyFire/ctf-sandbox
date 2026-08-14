using ctf_sandbox.Models;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class AuthenticationEndpoint : Endpoint
{
    public AuthenticationEndpoint(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<string> Authenticate(string username, string password)
    {
        var token = await PostAsyncAndEnsureSuccess<LoginRequest, string>("auth", new LoginRequest
        {
            Username = username,
            Password = password
        });
        return token;
    }
}