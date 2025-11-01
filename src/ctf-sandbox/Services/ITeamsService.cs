using ctf_sandbox.Areas.CTF.Models;

namespace ctf_sandbox.Services;

public interface ITeamsService
{
    Task<IEnumerable<Team>> GetTeamsForUserAsync(string userId);
}
