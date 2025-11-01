using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Data;
using Microsoft.EntityFrameworkCore;

namespace ctf_sandbox.Services;

public class TeamsService : ITeamsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TeamsService> _logger;

    public TeamsService(
        ApplicationDbContext context,
        ILogger<TeamsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Team>> GetTeamsForUserAsync(string userId)
    {
        var teams = await _context.Teams
            .Include(t => t.Owner)
            .Include(t => t.Members)
                .ThenInclude(m => m.User)
            .Where(t => t.OwnerId == userId || t.Members.Any(m => m.UserId == userId && !m.IsInvitePending))
            .ToListAsync();

        return teams;
    }
}
