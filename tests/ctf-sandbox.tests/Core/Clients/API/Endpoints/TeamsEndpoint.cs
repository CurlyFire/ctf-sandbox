using ctf_sandbox.Areas.CTF.Models;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class TeamsEndpoint : Endpoint
{

    public TeamsEndpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient) : base(jsonHttpClient)
    {
    }

    public async Task<Result<Team, ValidationProblemDetails>> CreateTeam(string? teamName, string memberCount, string jwt)
    {
        var createTeamRequest = new
        {
            Name = teamName,
            MemberCount = memberCount
        };

        return await JsonHttpClient.PostAsync<Team>("teams", createTeamRequest, jwt);
    }

    public async Task<Result<Team, ValidationProblemDetails>> CreateTeam(string? teamName, uint memberCount, string jwt)
    {
        return await CreateTeam(teamName, memberCount.ToString(), jwt);
    }

    public async Task<Result<IEnumerable<Team>, ValidationProblemDetails>> GetTeams(string jwt)
    {
        return await JsonHttpClient.GetAsync<IEnumerable<Team>>("teams", jwt);
    }

    public async Task<Result<Team, ValidationProblemDetails>> UpdateTeam(string teamId, string teamName, string? description, string memberCount, string jwt)
    {
        var updateRequest = new 
        {
            Name = teamName,
            Description = description,
            MemberCount = memberCount
        };

        return await JsonHttpClient.PutAsync<Team>($"teams/{teamId}", updateRequest, jwt);
    }

    public async Task<Result<Team, ValidationProblemDetails>> UpdateTeam(int teamId, string teamName, string? description, uint memberCount, string jwt)
    {
        return await UpdateTeam(teamId.ToString(), teamName, description, memberCount.ToString(), jwt);
    }
}