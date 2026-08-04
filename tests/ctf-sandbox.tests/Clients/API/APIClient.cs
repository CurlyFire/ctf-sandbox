using System.IdentityModel.Tokens.Jwt;
using ctf_sandbox.tests.Clients.API.Endpoints;

namespace ctf_sandbox.tests.Clients.API;

public class APIClient : HealthyHttpClient
{
    private readonly AuthenticationEndpoint _authenticationEndpoint;
    private readonly AccountEndpoint _accountEndpoint;
    private readonly TeamsEndpoint _teamsEndpoint;
    private readonly IpInfoEndpoint _ipInfoEndpoint;
        public APIClient(AuthenticationEndpoint authenticationEndpoint,
        AccountEndpoint accountEndpoint,
        TeamsEndpoint teamsEndpoint,
        IpInfoEndpoint ipInfoEndpoint,
        HttpClient httpClient) : base(httpClient)
    {
        _authenticationEndpoint = authenticationEndpoint;
        _accountEndpoint = accountEndpoint;
        _teamsEndpoint = teamsEndpoint;
        _ipInfoEndpoint = ipInfoEndpoint;
    }

    public AuthenticationEndpoint Authentication => _authenticationEndpoint;
    public AccountEndpoint Account => _accountEndpoint;
    public TeamsEndpoint Teams => _teamsEndpoint;
    public IpInfoEndpoint IpInfo => _ipInfoEndpoint;
    public JwtSecurityToken? UserJwtSecurityToken
    {
        get
        {
            if (HttpClient.DefaultRequestHeaders.Authorization?.Parameter is string token)
            {
                var handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(token);
            }
            return null;
        }
    }
}
