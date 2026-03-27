namespace Workhours.Api.Models.TimeEntries;

public sealed record TimeEntryResponse(
    Guid Id,
    Guid WorkweekId,
    DateOnly EntryDate,
    string ProjectName,
    decimal Hours,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);