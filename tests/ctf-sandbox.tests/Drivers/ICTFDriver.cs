using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests.Drivers;

public interface ICTFDriver
{
    Task<Emails> CheckEmails();
    Task<bool> CreateAccount(string email, string password);
    Task SignIn(string email, string password);
    Task CreateTeam(string teamName);
    Task<bool> IsTeamVisible(string teamName);
    Task<bool> IsUserSignedIn(string email);
}
