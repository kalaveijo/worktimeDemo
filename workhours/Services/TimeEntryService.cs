using Microsoft.EntityFrameworkCore;
using Workhours.Application.Exceptions;
using Workhours.Domain;
using Workhours.Infrastructure.Data;

namespace Workhours.Services;

public sealed class TimeEntryService(WorkhoursDbContext dbContext) : ITimeEntryService
{
    public async Task<IReadOnlyList<TimeEntry>> GetEntriesAsync(string userId, Guid workweekId, CancellationToken cancellationToken)
    {
        await EnsureWorkweekExistsAsync(userId, workweekId, cancellationToken);

        return await dbContext.TimeEntries
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.WorkweekId == workweekId)
            .OrderBy(x => x.EntryDate)
            .ThenBy(x => x.ProjectName)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimeEntry> AddEntryAsync(string userId, Guid workweekId, DateOnly entryDate, string projectName, decimal hours, CancellationToken cancellationToken)
    {
        var workweek = await EnsureWorkweekExistsAsync(userId, workweekId, cancellationToken);
        ValidateEntry(workweek, entryDate, projectName, hours);

        var normalizedProjectName = projectName.Trim();
        var exists = await dbContext.TimeEntries.AnyAsync(
            x => x.UserId == userId
                && x.WorkweekId == workweekId
                && x.EntryDate == entryDate
                && x.ProjectName == normalizedProjectName,
            cancellationToken);

        if (exists)
        {
            throw new ConflictException("A time entry already exists for this project and date.");
        }

        var now = DateTimeOffset.UtcNow;
        var timeEntry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkweekId = workweekId,
            EntryDate = entryDate,
            ProjectName = normalizedProjectName,
            Hours = hours,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        workweek.UpdatedAtUtc = now;
        dbContext.TimeEntries.Add(timeEntry);
        await dbContext.SaveChangesAsync(cancellationToken);
        return timeEntry;
    }

    public async Task<TimeEntry> UpdateEntryAsync(string userId, Guid workweekId, Guid entryId, decimal hours, CancellationToken cancellationToken)
    {
        if (hours <= 0 || hours > 24)
        {
            throw new ValidationException("Hours must be greater than 0 and less than or equal to 24.");
        }

        var timeEntry = await dbContext.TimeEntries
            .SingleOrDefaultAsync(x => x.Id == entryId && x.WorkweekId == workweekId && x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Time entry was not found.");

        timeEntry.Hours = hours;
        timeEntry.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return timeEntry;
    }

    public async Task DeleteEntryAsync(string userId, Guid workweekId, Guid entryId, CancellationToken cancellationToken)
    {
        var timeEntry = await dbContext.TimeEntries
            .SingleOrDefaultAsync(x => x.Id == entryId && x.WorkweekId == workweekId && x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Time entry was not found.");

        dbContext.TimeEntries.Remove(timeEntry);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Workweek> EnsureWorkweekExistsAsync(string userId, Guid workweekId, CancellationToken cancellationToken)
    {
        return await dbContext.Workweeks
            .SingleOrDefaultAsync(x => x.Id == workweekId && x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Workweek was not found.");
    }

    private static void ValidateEntry(Workweek workweek, DateOnly entryDate, string projectName, decimal hours)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            throw new ValidationException("Project name is required.");
        }

        if (entryDate < workweek.WeekStart || entryDate > workweek.WeekEnd)
        {
            throw new ValidationException("Entry date must fall within the selected workweek.");
        }

        if (hours <= 0 || hours > 24)
        {
            throw new ValidationException("Hours must be greater than 0 and less than or equal to 24.");
        }
    }
}