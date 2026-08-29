using System.IdentityModel.Tokens.Jwt;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Clients.API;
using Microsoft.AspNetCore.Mvc;

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

    public async Task<Result<VoidValue, SystemError>> CreateAccount(string email, string password)
    {
        return await _apiClient.Account.CreateAccount(email, password).MapErrorAsync(MapError);
    }

    public async Task<Result<int, SystemError>> CreateTeam(string? teamName, uint memberCount = 4)
    {
        return await _apiClient.Teams.CreateTeam(teamName, memberCount, _jwt).MapErrorAsync(MapError);
    }

    public async Task<Result<VoidValue, SystemError>> UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        var result = await _apiClient.Teams.GetTeams(_jwt);
        if (result.IsSuccess)
        {
            var team = result.Value.FirstOrDefault(t => t.Name == oldTeamName);
            if (team != null)
            {
                await _apiClient.Teams.UpdateTeam(team.Id, newTeamName, newDescription, memberCount ?? team.MemberCount, _jwt);
            }
            else
            {
                return Result.Failure(SystemError.Of($"Team '{oldTeamName}' not found"));
            }
        }
        else
        {
            return Result.Failure(MapError(result.Error));
        }

        return Result.Success<SystemError>();
    }

    public async Task<Result<IpInfo, SystemError>> GetIpInfo(string ipAddress)
    {
        return await _apiClient.IpInfo.GetIpInfo(ipAddress, _jwt).MapErrorAsync(MapError);
    }

    public async Task<Result<Team?, SystemError>> GetTeam(string teamName)
    {
        return await _apiClient.Teams.GetTeams(_jwt).MapErrorAsync(MapError).MapAsync(teams => teams.FirstOrDefault(t => t.Name == teamName));
    }
    public Task<Result<bool, SystemError>> IsUserSignedIn(string email)
    {
        var decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(_jwt);
        var isSignedIn = decodedJwt.Claims.Any(c => c.Type == "email" && c.Value == email);
        return Task.FromResult(Result.Success<SystemError>().Map(_ => isSignedIn));
    }
    
    public async Task<Result<VoidValue, SystemError>> SignIn(string email, string password)
    {
        var result = await _apiClient.Authentication.Authenticate(email, password);
        if (result.IsSuccess)
        {
            _jwt = result.Value;
            return Result.Success<SystemError>();
        }
        else
        {
            return Result.Failure(MapError(result.Error));
        }
    }

    public async Task<Result<VoidValue, SystemError>> GoToCTF()
    {
        return await _apiClient.Health.CheckHealth().MapErrorAsync(MapError);
    }

    private static SystemError MapError(ValidationProblemDetails problemDetail)
    {
        var message = problemDetail.Detail ?? "Request failed";
        if (problemDetail.Errors != null && problemDetail.Errors.Count > 0)
        {
            var fieldErrors = problemDetail.Errors
                .Select(e => new SystemError.FieldError(e.Key ?? "unknown", e.Value.ToString() ?? string.Empty))
                .ToList();
            return SystemError.Of(message, fieldErrors.AsReadOnly());
        }
        return SystemError.Of(message);
    }


}
