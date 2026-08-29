using ctf_sandbox.tests.Core.Clients.API.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API;

public class APIClient
{
    private readonly AuthenticationEndpoint _authenticationEndpoint;
    private readonly AccountEndpoint _accountEndpoint;
    private readonly TeamsEndpoint _teamsEndpoint;
    private readonly IpInfoEndpoint _ipInfoEndpoint;
    private readonly HealthEndpoint _healthEndpoint;
        public APIClient(HttpClient httpClient)
    {
        var jsonHttpClient = new JsonHttpClient<ValidationProblemDetails>(httpClient);
        _authenticationEndpoint = new AuthenticationEndpoint(jsonHttpClient);
        _accountEndpoint = new AccountEndpoint(jsonHttpClient);
        _teamsEndpoint = new TeamsEndpoint(jsonHttpClient);
        _ipInfoEndpoint = new IpInfoEndpoint(jsonHttpClient);
        _healthEndpoint = new HealthEndpoint(jsonHttpClient);;
    }

    public AuthenticationEndpoint Authentication => _authenticationEndpoint;
    public AccountEndpoint Account => _accountEndpoint;
    public TeamsEndpoint Teams => _teamsEndpoint;
    public IpInfoEndpoint IpInfo => _ipInfoEndpoint;
    public HealthEndpoint Health => _healthEndpoint;
}
