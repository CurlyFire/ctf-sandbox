using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;

namespace ctf_sandbox.tests.Core.Drivers.CTF;

public interface ICTFDriver
{
    Task<Result<VoidValue, SystemError>> CreateAccount(string email, string password);
    Task<Result<VoidValue, SystemError>> SignIn(string email, string password);
    Task<Result<int, SystemError>> CreateTeam(string? teamName, uint memberCount = 4);
    Task<Result<VoidValue, SystemError>> UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null);
    Task<Result<Team?, SystemError>> GetTeam(string teamName);
    Task<Result<bool, SystemError>> IsUserSignedIn(string email);
    Task<Result<IpInfo, SystemError>> GetIpInfo(string ipAddress);
    Task<Result<VoidValue, SystemError>> GoToCTF();
}
