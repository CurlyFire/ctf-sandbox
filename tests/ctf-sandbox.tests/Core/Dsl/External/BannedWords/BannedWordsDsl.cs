using ctf_sandbox.tests.Core.Dsl.UseCases;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;
using ctf_sandbox.tests.Core.Dsl.External.BannedWords.UseCases;

namespace ctf_sandbox.tests.Core.Dsl.External.BannedWords;

public class BannedWordsDsl
{
    private readonly IBannedWordsDriver _bannedWordsDriver;
    private readonly UseCaseFactory<IBannedWordsDriver> _useCaseFactory;

    public BannedWordsDsl(IBannedWordsDriver bannedWordsDriver, IServiceProvider serviceProvider)
    {
        _bannedWordsDriver = bannedWordsDriver;
        _useCaseFactory = new UseCaseFactory<IBannedWordsDriver>(bannedWordsDriver, serviceProvider);
    }

    public GoToBannedWords GoToBannedWords() => _useCaseFactory.Create<GoToBannedWords>();

    public async Task CreateBannedWord(string word)
    {
        await _bannedWordsDriver.CreateBannedWordAsync(word);
    }
}