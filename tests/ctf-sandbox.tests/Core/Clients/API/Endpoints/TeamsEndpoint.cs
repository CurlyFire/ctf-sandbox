using ctf_sandbox.Areas.CTF.Models;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class TeamsEndpoint : Endpoint
{

    public TeamsEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }

    public async Task<Result<int, ValidationProblemDetails>> CreateTeam(string? teamName, string memberCount, string jwt)
    {
        var createTeamRequest = new
        {
            Name = teamName,
            MemberCount = memberCount
        };

        var result = await JsonHttpClient.PostAsync<Team>("teams", createTeamRequest, jwt);
        if (result.IsSuccess)
        {
            return Result<int, ValidationProblemDetails>.Success(result.Value.Id);
        }
        else
        {
            return Result<int, ValidationProblemDetails>.Failure(result.Error);
        }
    }

    public async Task<Result<int, ValidationProblemDetails>> CreateTeam(string? teamName, uint memberCount, string jwt)
    {
        return await CreateTeam(teamName, memberCount.ToString(), jwt);
    }

    public async Task<Result<IEnumerable<Team>, ValidationProblemDetails>> GetTeams(string jwt)
    {
        return await JsonHttpClient.GetAsync<IEnumerable<Team>>("teams", jwt);
    }

    public async Task<Result<VoidValue, ValidationProblemDetails>> UpdateTeam(string teamId, string teamName, string? description, string memberCount, string jwt)
    {
        var updateRequest = new 
        {
            Name = teamName,
            Description = description,
            MemberCount = memberCount
        };

        return await JsonHttpClient.PutAsync($"teams/{teamId}", updateRequest, jwt);
    }

    public async Task<Result<VoidValue, ValidationProblemDetails>> UpdateTeam(int teamId, string teamName, string? description, uint memberCount, string jwt)
    {
        return await UpdateTeam(teamId.ToString(), teamName, description, memberCount.ToString(), jwt);
    }
}