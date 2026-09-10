using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class BannedWordsRealClient : HealthyTcpClient
{
    protected JsonHttpClient<ValidationProblemDetails> JsonHttpClient {get;}

    public BannedWordsRealClient(HttpClient httpClient) : base(httpClient.BaseAddress!)
    {
        JsonHttpClient = new JsonHttpClient<ValidationProblemDetails>(httpClient);
    }

    public async Task<Result<VoidValue, ValidationProblemDetails>> CreateBannedWordAsync(string word)
    {
        return await JsonHttpClient.PostAsync("/BannedWords", new { Word = word });
    }
}