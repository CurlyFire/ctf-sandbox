
using System.Net.Http.Json;
using System.Text.Json;

namespace ctf_sandbox.tests.Drivers.ExternalSystems;

public class APIBannedWordsDriver : IBannedWordsDriver
{
    private readonly HttpClient _httpClient;

    public APIBannedWordsDriver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task CreateBannedWordAsync(string word)
    {
        var response = await _httpClient.PostAsJsonAsync("/BannedWords", new { Word = word });
        response.EnsureSuccessStatusCode();
    }
}
