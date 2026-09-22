using ctf_sandbox.tests.Core;
using ctf_sandbox.tests.Core.Clients.ExternalSystems;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class APIBannedWordsDriver : IBannedWordsDriver
{
    private readonly BannedWordsRealClient _client;

    public APIBannedWordsDriver(BannedWordsRealClient client)
    {
        _client = client;
    }

    public async Task<Result<VoidValue, SystemError>> GoToBannedWords()
    {
        var isHealthy = await _client.IsHealthy();
        return isHealthy
            ? Result.Success<SystemError>()
            : Result.Failure<SystemError>(SystemError.Of("BannedWords service is not healthy"));
    }

    public async Task CreateBannedWordAsync(string word)
    {
        await _client.CreateBannedWordAsync(word);
    }
}
