using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ctf_sandbox.Areas.CTF.Models;

namespace ctf_sandbox.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;

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
    }
}
