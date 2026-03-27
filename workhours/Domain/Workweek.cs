namespace Workhours.Domain;

public class Workweek
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }

    public DateOnly WeekStart { get; set; }

    public DateOnly WeekEnd { get; set; }

    public bool IsFinalized { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
}