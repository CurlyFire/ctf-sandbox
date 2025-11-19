using ctf_sandbox.tests.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Dsl;

public class BannedWords
{
    private readonly IBannedWordsDriver _bannedWordsDriver;

    public BannedWords(IBannedWordsDriver bannedWordsDriver)
    {
        _bannedWordsDriver = bannedWordsDriver;
    }

    public async Task CreateBannedWord(string word)
    {
        await _bannedWordsDriver.CreateBannedWordAsync(word);
    }
}