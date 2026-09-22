using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.BannedWords;

public class BannedWordsDsl
{
    private readonly IBannedWordsDriver _bannedWordsDriver;

    public BannedWordsDsl(IBannedWordsDriver bannedWordsDriver)
    {
        _bannedWordsDriver = bannedWordsDriver;
    }

    public async Task CreateBannedWord(string word)
    {
        await _bannedWordsDriver.CreateBannedWordAsync(word);
    }
}