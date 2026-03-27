using Workhours.Domain;

namespace Workhours.Services;

public interface ITimeEntryService
{
    Task<IReadOnlyList<TimeEntry>> GetEntriesAsync(string userId, Guid workweekId, CancellationToken cancellationToken);

    Task<TimeEntry> AddEntryAsync(string userId, Guid workweekId, DateOnly entryDate, string projectName, decimal hours, CancellationToken cancellationToken);

    Task<TimeEntry> UpdateEntryAsync(string userId, Guid workweekId, Guid entryId, decimal hours, CancellationToken cancellationToken);

    Task DeleteEntryAsync(string userId, Guid workweekId, Guid entryId, CancellationToken cancellationToken);
}