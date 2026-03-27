using System.ComponentModel.DataAnnotations;

namespace Workhours.Api.Models.TimeEntries;

public sealed class CreateTimeEntryRequest
{
    [Required]
    public DateOnly EntryDate { get; init; }

    [Required]
    [MaxLength(200)]
    public string ProjectName { get; init; } = string.Empty;

    [Range(0.01, 24)]
    public decimal Hours { get; init; }
}