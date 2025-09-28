using ctf_sandbox.tests.Fixtures.Dsl;

namespace ctf_sandbox.tests.Fixtures.Drivers;

public interface ICTFDriver
{
    Task<EmailsDsl> CheckEmails();
    Task<bool> CreateAccount(string email, string password);
    Task SignIn(string email, string password);
    Task CreateTeam(string teamName);
    Task<bool> IsTeamVisible(string teamName);
}
