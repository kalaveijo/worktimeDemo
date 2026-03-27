namespace Workhours.Api.Models.Workweeks;

public sealed record WorkweekResponse(
    Guid Id,
    DateOnly WeekStart,
    DateOnly WeekEnd,
    bool IsFinalized,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);