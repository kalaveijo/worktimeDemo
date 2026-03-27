using Microsoft.EntityFrameworkCore;
using Workhours.Application.Exceptions;
using Workhours.Domain;
using Workhours.Infrastructure.Data;

namespace Workhours.Services;

public sealed class WeekService(WorkhoursDbContext dbContext) : IWeekService
{
    public async Task<Workweek> GetWeekByDateAsync(string userId, DateOnly? date, CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var (weekStart, weekEnd) = WeekCalculator.GetWeekRange(targetDate);

        var workweek = await dbContext.Workweeks
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.WeekStart == weekStart, cancellationToken);

        if (workweek is not null)
        {
            return workweek;
        }

        var now = DateTimeOffset.UtcNow;
        workweek = new Workweek
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            IsFinalized = false,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        dbContext.Workweeks.Add(workweek);
        await dbContext.SaveChangesAsync(cancellationToken);
        return workweek;
    }

    public async Task<Workweek> SetFinalizedStateAsync(string userId, Guid workweekId, bool isFinalized, CancellationToken cancellationToken)
    {
        var workweek = await dbContext.Workweeks
            .SingleOrDefaultAsync(x => x.Id == workweekId && x.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Workweek was not found.");

        workweek.IsFinalized = isFinalized;
        workweek.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return workweek;
    }
}