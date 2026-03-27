using Microsoft.AspNetCore.Identity;

namespace Workhours.Domain;

public class ApplicationUser : IdentityUser
{
    public ICollection<Workweek> Workweeks { get; set; } = [];

    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
}