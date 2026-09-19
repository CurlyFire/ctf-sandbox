using System.IdentityModel.Tokens.Jwt;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Clients.API;

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
        return await _apiClient.Account.CreateAccount(email, password).MapErrorAsync(ValidationProblemDetailsExtensions.MapError);
    }

    public async Task<Result<Team, SystemError>> CreateTeam(string? teamName, uint memberCount = 4)
    {
        return await _apiClient.Teams.CreateTeam(teamName, memberCount, _jwt).MapErrorAsync(ValidationProblemDetailsExtensions.MapError);
    }

    public async Task<Result<Team, SystemError>> UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        var result = await _apiClient.Teams.GetTeams(_jwt);
        if (result.IsSuccess)
        {
            var team = result.Value.FirstOrDefault(t => t.Name == oldTeamName);
            if (team != null)
            {
                var updateResult = await _apiClient.Teams.UpdateTeam(team.Id, newTeamName, newDescription, memberCount ?? team.MemberCount, _jwt);
                if (!updateResult.IsSuccess)
                {
                    return Result<Team, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(updateResult.Error));
                }
                return Result<Team, SystemError>.Success(updateResult.Value);
            }
            else
            {
                return Result<Team, SystemError>.Failure(SystemError.Of($"Team '{oldTeamName}' not found"));
            }
        }
        else
        {
            return  Result<Team, SystemError>.Failure(ValidationProblemDetailsExtensions.MapError(result.Error));
        }
    }

    public async Task<Result<IpInfo, SystemError>> GetIpInfo(string ipAddress)
    {
        return await _apiClient.IpInfo.GetIpInfo(ipAddress, _jwt).MapErrorAsync(ValidationProblemDetailsExtensions.MapError);
    }

    public async Task<Result<Team?, SystemError>> GetTeam(string teamName)
    {
        return await _apiClient.Teams.GetTeams(_jwt).MapErrorAsync(ValidationProblemDetailsExtensions.MapError).MapAsync(teams => teams.FirstOrDefault(t => t.Name == teamName));
    }

    public async Task<Result<VoidValue, SystemError>> SignIn(string? email, string? password)
    {
        var result = await _apiClient.Authentication.Authenticate(email, password);
        if (result.IsSuccess)
        {
            _jwt = result.Value;
            return Result.Success<SystemError>();
        }
        else
        {
            return Result.Failure(ValidationProblemDetailsExtensions.MapError(result.Error));
        }
    }

    public async Task<Result<VoidValue, SystemError>> GoToCTF()
    {
        return await _apiClient.Health.CheckHealth().MapErrorAsync(ValidationProblemDetailsExtensions.MapError);
    }
}
