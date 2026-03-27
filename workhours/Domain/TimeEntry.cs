namespace Workhours.Domain;

public class TimeEntry
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }

    public Guid WorkweekId { get; set; }

    public DateOnly EntryDate { get; set; }

    public required string ProjectName { get; set; }

    public decimal Hours { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ApplicationUser? User { get; set; }

    public Workweek? Workweek { get; set; }
}