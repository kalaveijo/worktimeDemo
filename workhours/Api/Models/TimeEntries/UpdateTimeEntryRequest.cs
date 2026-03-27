using System.ComponentModel.DataAnnotations;

namespace Workhours.Api.Models.TimeEntries;

public sealed class UpdateTimeEntryRequest
{
    [Range(0.01, 24)]
    public decimal Hours { get; init; }
}