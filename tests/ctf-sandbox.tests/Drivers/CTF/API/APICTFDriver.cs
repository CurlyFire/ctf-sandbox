using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Clients.API;

namespace ctf_sandbox.tests.Drivers.CTF.API;

public class APICTFDriver : ICTFDriver
{
    private readonly APIClient _apiClient;


    public APICTFDriver(APIClient apiClient)
    {
        _apiClient = apiClient;
        
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        var accountEndpoint = _apiClient.Account;
        try
        {
            await accountEndpoint.CreateAccount(email, password);
            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    public async Task<string?> CreateTeam(string? teamName, uint memberCount = 4)
    {
        try
        {
            await _apiClient.Teams.CreateTeam("teams",4);
            return null;
        }
        catch (HttpRequestException exc)
        {
            return "dude, check tes affaires";
        }
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        var teams = await _apiClient.Teams.GetTeams();

        var team = teams?.FirstOrDefault(t => t.Name == oldTeamName);
        
        if (team == null)
        {
            throw new InvalidOperationException($"Team '{oldTeamName}' not found");
        }


    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        return await _apiClient.IpInfo.GetIpInfo(ipAddress);
    }

    public async Task<Team?> GetTeam(string teamName)
    {
        var teams = await _apiClient.Teams.GetTeams();

       
        return teams.FirstOrDefault(t => t.Name == teamName);
    }

    public Task<bool> IsUserSignedIn(string email)
    {
        var decodedJwt = _apiClient.UserJwtSecurityToken;
        return Task.FromResult(decodedJwt.Claims.Any(c => c.Type == "email" && c.Value == email));
    }

    public Task SignIn(string email, string password)
    {
        throw new NotImplementedException();
    }
}
