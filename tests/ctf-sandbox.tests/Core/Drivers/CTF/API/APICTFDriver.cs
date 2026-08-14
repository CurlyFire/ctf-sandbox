using System.IdentityModel.Tokens.Jwt;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Clients.API;
using ctf_sandbox.tests.Core.Clients.API.Endpoints;

namespace ctf_sandbox.tests.Core.Drivers.CTF.API;

public class APICTFDriver : ICTFDriver
{
    private readonly APIClient _apiClient;

    private string _jwt;


    public APICTFDriver(APIClient apiClient)
    {
        _apiClient = apiClient;
        _jwt = string.Empty;
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
            await _apiClient.Teams.CreateTeam(teamName, memberCount, _jwt);
            return null;
        }
        catch (UnsuccessfulHttpResponseException exc)
        {
            var validationProblemDetails = await exc.Response.GetValidationProblemDetails();
            //TODO : Extract helper function to format validation problem details into a string
            if (validationProblemDetails != null)
            {
                var errorMessages = validationProblemDetails.Errors.SelectMany(e => e.Value);
                if (validationProblemDetails.Detail != null)
                {
                    return string.Join("; ", validationProblemDetails.Detail, errorMessages);
                }
                else
                {
                    return string.Join("; ", errorMessages);
                }
            }
            else
            {
                throw new InvalidOperationException("Failed to create team, and no validation problem details were provided.");
            }
        }
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        var teams = await _apiClient.Teams.GetTeams(_jwt);

        var team = teams?.FirstOrDefault(t => t.Name == oldTeamName);
        
        if (team == null)
        {
            throw new InvalidOperationException($"Team '{oldTeamName}' not found");
        }
        await _apiClient.Teams.UpdateTeam(team.Id, newTeamName, newDescription, memberCount ?? team.MemberCount, _jwt);
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        return await _apiClient.IpInfo.GetIpInfo(ipAddress, _jwt);
    }

    public async Task<Team?> GetTeam(string teamName)
    {
        var teams = await _apiClient.Teams.GetTeams(_jwt);
        return teams.FirstOrDefault(t => t.Name == teamName);
    }

    public async Task ConfirmUserIsSignedIn(string email)
    {
        var decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(_jwt);
        Assert.Contains(decodedJwt.Claims, c => c.Type == "email" && c.Value == email);
    }

    public async Task SignIn(string email, string password)
    {
        _jwt = await _apiClient.Authentication.Authenticate(email, password);
    }

    public async Task ConfirmIsUpAndRunning()
    {
        Assert.True(await _apiClient.Health.IsHealthy(), "API is not healthy");
    }
}
