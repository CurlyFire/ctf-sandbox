namespace ctf_sandbox.Services;

public interface IBannedWordsService
{
    Task<bool> ContainsBannedWordAsync(string text);
}
