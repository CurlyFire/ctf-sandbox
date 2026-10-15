using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;

namespace ctf_sandbox.tests.Core.Drivers.CTF;

public interface ICTFDriver
{
    Task<Result<VoidValue, SystemError>> CreateAccount(string email, string password);
    Task<Result<VoidValue, SystemError>> SignIn(string? email, string? password);
    Task<Result<GetTeamResponse, SystemError>> CreateTeam(string? teamName, uint memberCount = 4);
    Task<Result<GetTeamResponse, SystemError>> CreateTeam(string? teamName, string memberCount);
    Task<Result<GetTeamResponse, SystemError>> UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null);
    Task<Result<GetTeamResponse?, SystemError>> GetTeam(string teamName);
    Task<Result<GetIpInfoResponse, SystemError>> GetIpInfo(string ipAddress);
    Task<Result<VoidValue, SystemError>> GoToCTF();
}
