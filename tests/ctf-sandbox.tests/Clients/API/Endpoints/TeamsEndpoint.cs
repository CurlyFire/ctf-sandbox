using ctf_sandbox.Areas.CTF.Models;

namespace ctf_sandbox.tests.Clients.API.Endpoints;

public class TeamsEndpoint : Endpoint
{
    public TeamsEndpoint(HttpClient httpClient) : base(httpClient)
    {
    }


    public async Task CreateTeam(string teamName, string memberCount)
    {
        await PostAsyncAndEnsureSuccess("teams", new
        {
            Name = teamName,
            MemberCount = memberCount
        });        
    }
    public async Task CreateTeam(string teamName, uint memberCount = 4)
    {
        await CreateTeam(teamName, memberCount.ToString());
    }

    public async Task<IEnumerable<Team>> GetTeams()
    {
        var teams = await GetAsyncAndEnsureSuccess<List<Team>>("teams");
        return teams;
    }

    public async Task UpdateTeam(string teamId, string teamName, string? description, string memberCount)
    {
        var updateRequest = new 
        {
            Name = teamName,
            Description = description,
            MemberCount = memberCount
        };

        await PutAsyncAndEnsureSuccess($"teams/{teamId}", updateRequest);

    }

    public async Task UpdateTeam(int teamId, string teamName, string? description, uint memberCount)
    {
        await UpdateTeam(teamId.ToString(), teamName, description, memberCount.ToString());
    }
}