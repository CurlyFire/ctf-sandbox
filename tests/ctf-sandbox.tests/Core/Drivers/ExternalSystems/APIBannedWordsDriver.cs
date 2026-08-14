
using ctf_sandbox.tests.Core.Clients.ExternalSystems;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

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
