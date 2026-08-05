
using ctf_sandbox.tests.Clients.ExternalSystems;

namespace ctf_sandbox.tests.Drivers.ExternalSystems;

public class APIBannedWordsDriver : IBannedWordsDriver
{
    private readonly BannedWordsRealClient _client;

    public APIBannedWordsDriver(BannedWordsRealClient client)
    {
        _client = client;
    }
    public async Task CreateBannedWordAsync(string word)
    {
        await _client.CreateBannedWordAsync(word);
    }
}
