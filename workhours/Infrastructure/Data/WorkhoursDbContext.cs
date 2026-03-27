using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workhours.Domain;

namespace Workhours.Infrastructure.Data;

public class WorkhoursDbContext(DbContextOptions<WorkhoursDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Workweek> Workweeks => Set<Workweek>();

    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Workweek>(entity =>
        {
            entity.ToTable("Workweeks");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => new { x.UserId, x.WeekStart }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.Workweeks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TimeEntry>(entity =>
        {
            entity.ToTable("TimeEntries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.ProjectName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Hours).HasPrecision(5, 2);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.UpdatedAtUtc).IsRequired();
            entity.HasIndex(x => new { x.UserId, x.WorkweekId, x.EntryDate, x.ProjectName }).IsUnique();

            entity.HasOne(x => x.User)
                .WithMany(x => x.TimeEntries)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Workweek)
                .WithMany(x => x.TimeEntries)
                .HasForeignKey(x => x.WorkweekId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}