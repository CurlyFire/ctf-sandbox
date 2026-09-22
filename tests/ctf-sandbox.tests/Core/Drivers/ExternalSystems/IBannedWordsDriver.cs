using ctf_sandbox.tests.Core;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public interface IBannedWordsDriver
{
    Task<Result<VoidValue, SystemError>> GoToBannedWords();
    Task CreateBannedWordAsync(string word);
}
