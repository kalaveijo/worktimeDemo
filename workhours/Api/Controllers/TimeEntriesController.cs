using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workhours.Api.Extensions;
using Workhours.Api.Models.TimeEntries;
using Workhours.Domain;
using Workhours.Services;

namespace Workhours.Api.Controllers;

[ApiController]
[Route("api/weeks/{workweekId:guid}/entries")]
[Authorize]
public sealed class TimeEntriesController(ITimeEntryService timeEntryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TimeEntryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TimeEntryResponse>>> GetEntriesAsync(Guid workweekId, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var entries = await timeEntryService.GetEntriesAsync(userId, workweekId, cancellationToken);
        return Ok(entries.Select(MapResponse).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(TimeEntryResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TimeEntryResponse>> CreateEntryAsync(
        Guid workweekId,
        [FromBody] CreateTimeEntryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var entry = await timeEntryService.AddEntryAsync(
            userId,
            workweekId,
            request.EntryDate,
            request.ProjectName,
            request.Hours,
            cancellationToken);

        return CreatedAtAction(nameof(GetEntriesAsync), new { workweekId }, MapResponse(entry));
    }

    [HttpPut("{entryId:guid}")]
    [ProducesResponseType(typeof(TimeEntryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeEntryResponse>> UpdateEntryAsync(
        Guid workweekId,
        Guid entryId,
        [FromBody] UpdateTimeEntryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var entry = await timeEntryService.UpdateEntryAsync(userId, workweekId, entryId, request.Hours, cancellationToken);
        return Ok(MapResponse(entry));
    }

    [HttpDelete("{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteEntryAsync(Guid workweekId, Guid entryId, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        await timeEntryService.DeleteEntryAsync(userId, workweekId, entryId, cancellationToken);
        return NoContent();
    }

    private static TimeEntryResponse MapResponse(TimeEntry timeEntry)
    {
        return new TimeEntryResponse(
            timeEntry.Id,
            timeEntry.WorkweekId,
            timeEntry.EntryDate,
            timeEntry.ProjectName,
            timeEntry.Hours,
            timeEntry.CreatedAtUtc,
            timeEntry.UpdatedAtUtc);
    }
}