using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Models;

namespace ctf_sandbox.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;
    public DbSet<Challenge> Challenges { get; set; } = null!;
    public DbSet<PlaintextChallenge> PlaintextChallenges { get; set; } = null!;
    public DbSet<Competition> Competitions { get; set; } = null!;
    public DbSet<CompetitionTeam> CompetitionTeams { get; set; } = null!;
    public DbSet<CompetitionChallenge> CompetitionChallenges { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Team>()
            .HasOne(t => t.Owner)
            .WithMany()
            .HasForeignKey(t => t.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TeamMember>()
            .HasOne(tm => tm.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(tm => tm.TeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TeamMember>()
            .HasOne(tm => tm.User)
            .WithMany()
            .HasForeignKey(tm => tm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
            
        // Challenge configuration
        builder.Entity<Challenge>()
            .HasDiscriminator(c => c.ChallengeType)
            .HasValue<PlaintextChallenge>("Plaintext");

        builder.Entity<Challenge>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Competition configuration
        builder.Entity<Competition>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CompetitionTeam>()
            .HasOne(ct => ct.Competition)
            .WithMany(c => c.Teams)
            .HasForeignKey(ct => ct.CompetitionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CompetitionTeam>()
            .HasOne(ct => ct.Team)
            .WithMany()
            .HasForeignKey(ct => ct.TeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CompetitionChallenge>()
            .HasOne(cc => cc.Competition)
            .WithMany(c => c.Challenges)
            .HasForeignKey(cc => cc.CompetitionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CompetitionChallenge>()
            .HasOne(cc => cc.Challenge)
            .WithMany()
            .HasForeignKey(cc => cc.ChallengeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
