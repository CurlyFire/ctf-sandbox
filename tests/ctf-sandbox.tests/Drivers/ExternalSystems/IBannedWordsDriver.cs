namespace ctf_sandbox.tests.Drivers.ExternalSystems;

public interface IBannedWordsDriver
{
    Task CreateBannedWordAsync(string word);
}
