using System.Net.Http.Json;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class BannedWordsRealClient : HealthyHttpClient
{
    public BannedWordsRealClient(HttpClient client) : base(client)
    {
    }

    public async Task CreateBannedWordAsync(string word)
    {
        var response = await HttpClient.PostAsJsonAsync("/BannedWords", new { Word = word });
        response.EnsureSuccessStatusCode();
    }
}