using ctf_sandbox.tests.Core.Clients.ExternalSystems.BannedWords;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class BannedWordsStubbedDriver : IBannedWordsDriver
{
    private readonly BannedWordsStubbedClient _client;

    public BannedWordsStubbedDriver(BannedWordsStubbedClient client)
    {
        _client = client;
    }

    public async Task<Result<VoidValue, SystemError>> GoToBannedWords()
    {
        return await _client.CheckHealth();
    }

    public async Task CreateBannedWordAsync(string word)
    {
        await _client.CreateBannedWordAsync(word);
    }
}
