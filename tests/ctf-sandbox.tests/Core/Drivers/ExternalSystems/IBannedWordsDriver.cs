namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public interface IBannedWordsDriver
{
    Task CreateBannedWordAsync(string word);
}
