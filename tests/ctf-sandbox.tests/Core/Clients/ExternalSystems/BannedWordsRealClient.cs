using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.ExternalSystems;

public class BannedWordsRealClient
{
    protected JsonHttpClient<ValidationProblemDetails> JsonHttpClient {get;}

    protected BannedWordsRealClient(HttpClient httpClient)
    {
        JsonHttpClient = new JsonHttpClient<ValidationProblemDetails>(httpClient);
    }

    public async Task<Result<VoidValue, ValidationProblemDetails>> CreateBannedWordAsync(string word)
    {
        return await JsonHttpClient.PostAsync("/BannedWords", new { Word = word });
    }
}