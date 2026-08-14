using ctf_sandbox.Areas.CTF.Models;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class TeamsEndpoint : Endpoint
{
    public TeamsEndpoint(HttpClient httpClient) : base(httpClient)
    {
    }


    public async Task CreateTeam(string teamName, string memberCount, string jwt)
    {
        await PostAsyncAndEnsureSuccess("teams", new
        {
            Name = teamName,
            MemberCount = memberCount
        }, jwt);        
    }
    public async Task CreateTeam(string teamName, uint memberCount, string jwt)
    {
        await CreateTeam(teamName, memberCount.ToString(), jwt);
    }

    public async Task<IEnumerable<Team>> GetTeams(string jwt)
    {
        var teams = await GetAsyncAndEnsureSuccess<List<Team>>("teams", jwt);
        return teams;
    }

    public async Task UpdateTeam(string teamId, string teamName, string? description, string memberCount, string jwt)
    {
        var updateRequest = new 
        {
            Name = teamName,
            Description = description,
            MemberCount = memberCount
        };

        await PutAsyncAndEnsureSuccess($"teams/{teamId}", updateRequest, jwt);

    }

    public async Task UpdateTeam(int teamId, string teamName, string? description, uint memberCount, string jwt)
    {
        await UpdateTeam(teamId.ToString(), teamName, description, memberCount.ToString(), jwt);
    }
}