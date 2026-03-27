using Workhours.Domain;

namespace Workhours.Services;

public interface IWeekService
{
    Task<Workweek> GetWeekByDateAsync(string userId, DateOnly? date, CancellationToken cancellationToken);

    Task<Workweek> SetFinalizedStateAsync(string userId, Guid workweekId, bool isFinalized, CancellationToken cancellationToken);
}