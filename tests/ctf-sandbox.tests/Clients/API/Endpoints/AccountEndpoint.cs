using ctf_sandbox.Models;

namespace ctf_sandbox.tests.Clients.API.Endpoints;

public class AccountEndpoint : Endpoint
{
    public AccountEndpoint(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task CreateAccount(string email, string password)
    {
        await PostAsyncAndEnsureSuccess("account", new RegisterAccountRequest
        {
            Email = email,
            Password = password
        });
    }
}