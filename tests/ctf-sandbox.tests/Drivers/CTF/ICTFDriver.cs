using ctf_sandbox.Models;
using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests.Drivers.CTF;

public interface ICTFDriver
{
    Task<bool> CreateAccount(string email, string password);
    Task SignIn(string email, string password);
    Task<string?> CreateTeam(string? teamName);
    Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null);
    Task<bool> IsTeamAvailable(string teamName);
    Task<bool> IsUserSignedIn(string email);
    Task<IpInfo> GetIpInfo(string ipAddress);
}
