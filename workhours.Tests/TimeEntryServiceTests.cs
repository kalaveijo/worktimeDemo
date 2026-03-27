using Microsoft.EntityFrameworkCore;
using Workhours.Application.Exceptions;
using Workhours.Domain;
using Workhours.Infrastructure.Data;
using Workhours.Services;
using Xunit;

namespace Workhours.Tests;

public class TimeEntryServiceTests
{
    [Fact]
    public async Task AddEntryAsync_RejectsEntryOutsideSelectedWeek()
    {
        await using var dbContext = CreateDbContext();
        var user = new ApplicationUser
        {
            Id = "user-1",
            UserName = "user@example.com",
            Email = "user@example.com",
            NormalizedUserName = "USER@EXAMPLE.COM",
            NormalizedEmail = "USER@EXAMPLE.COM"
        };

        dbContext.Users.Add(user);
        dbContext.Workweeks.Add(new Workweek
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            WeekStart = new DateOnly(2026, 3, 23),
            WeekEnd = new DateOnly(2026, 3, 29),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var service = new TimeEntryService(dbContext);
        var workweekId = dbContext.Workweeks.Single().Id;

        await Assert.ThrowsAsync<ValidationException>(() => service.AddEntryAsync(
            user.Id,
            workweekId,
            new DateOnly(2026, 3, 30),
            "Project A",
            4,
            CancellationToken.None));
    }

    [Fact]
    public async Task GetEntriesAsync_ReturnsOnlyCurrentUsersEntries()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Workweeks.AddRange(
            new Workweek
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserId = "user-1",
                WeekStart = new DateOnly(2026, 3, 23),
                WeekEnd = new DateOnly(2026, 3, 29),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            },
            new Workweek
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UserId = "user-2",
                WeekStart = new DateOnly(2026, 3, 23),
                WeekEnd = new DateOnly(2026, 3, 29),
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            });

        dbContext.TimeEntries.AddRange(
            new TimeEntry
            {
                Id = Guid.NewGuid(),
                UserId = "user-1",
                WorkweekId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                EntryDate = new DateOnly(2026, 3, 23),
                ProjectName = "Project A",
                Hours = 8,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            },
            new TimeEntry
            {
                Id = Guid.NewGuid(),
                UserId = "user-2",
                WorkweekId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                EntryDate = new DateOnly(2026, 3, 23),
                ProjectName = "Project B",
                Hours = 7,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                UpdatedAtUtc = DateTimeOffset.UtcNow
            });

        await dbContext.SaveChangesAsync();

        var service = new TimeEntryService(dbContext);
        var entries = await service.GetEntriesAsync("user-1", Guid.Parse("11111111-1111-1111-1111-111111111111"), CancellationToken.None);

        Assert.Single(entries);
        Assert.Equal("Project A", entries[0].ProjectName);
    }

    private static WorkhoursDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<WorkhoursDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new WorkhoursDbContext(options);
    }
}