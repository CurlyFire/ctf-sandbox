using System.IdentityModel.Tokens.Jwt;
using ctf_sandbox.tests.Core.Clients.API.Endpoints;

namespace ctf_sandbox.tests.Core.Clients.API;

public class APIClient : HealthyHttpClient
{
    private readonly AuthenticationEndpoint _authenticationEndpoint;
    private readonly AccountEndpoint _accountEndpoint;
    private readonly TeamsEndpoint _teamsEndpoint;
    private readonly IpInfoEndpoint _ipInfoEndpoint;
    private readonly HealthEndpoint _healthEndpoint;
        public APIClient(AuthenticationEndpoint authenticationEndpoint,
        AccountEndpoint accountEndpoint,
        TeamsEndpoint teamsEndpoint,
        IpInfoEndpoint ipInfoEndpoint,
        HealthEndpoint healthEndpoint,
        HttpClient httpClient) : base(httpClient)
    {
        _authenticationEndpoint = authenticationEndpoint;
        _accountEndpoint = accountEndpoint;
        _teamsEndpoint = teamsEndpoint;
        _ipInfoEndpoint = ipInfoEndpoint;
        _healthEndpoint = healthEndpoint;
    }

    public AuthenticationEndpoint Authentication => _authenticationEndpoint;
    public AccountEndpoint Account => _accountEndpoint;
    public TeamsEndpoint Teams => _teamsEndpoint;
    public IpInfoEndpoint IpInfo => _ipInfoEndpoint;
    public HealthEndpoint Health => _healthEndpoint;
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
