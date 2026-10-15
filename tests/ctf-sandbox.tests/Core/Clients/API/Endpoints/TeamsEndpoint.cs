using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class TeamsEndpoint : Endpoint
{

    public TeamsEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }

    public async Task<Result<GetTeamResponse, ValidationProblemDetails>> CreateTeam(string? teamName, string memberCount, string jwt)
    {
        var createTeamRequest = new
        {
            Name = teamName,
            MemberCount = memberCount
        };

        return await JsonHttpClient.PostAsync<GetTeamResponse>("teams", createTeamRequest, jwt);
    }

    public async Task<Result<GetTeamResponse, ValidationProblemDetails>> CreateTeam(string? teamName, uint memberCount, string jwt)
    {
        return await CreateTeam(teamName, memberCount.ToString(), jwt);
    }

    public async Task<Result<IEnumerable<GetTeamResponse>, ValidationProblemDetails>> GetTeams(string jwt)
    {
        return await JsonHttpClient.GetAsync<IEnumerable<GetTeamResponse>>("teams", jwt);
    }

    public async Task<Result<GetTeamResponse, ValidationProblemDetails>> UpdateTeam(string teamId, string teamName, string? description, string memberCount, string jwt)
    {
        var updateRequest = new 
        {
            Name = teamName,
            Description = description,
            MemberCount = memberCount
        };

        return await JsonHttpClient.PutAsync<GetTeamResponse>($"teams/{teamId}", updateRequest, jwt);
    }

    public async Task<Result<GetTeamResponse, ValidationProblemDetails>> UpdateTeam(int teamId, string teamName, string? description, uint memberCount, string jwt)
    {
        return await UpdateTeam(teamId.ToString(), teamName, description, memberCount.ToString(), jwt);
    }
}