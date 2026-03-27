using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workhours.Api.Extensions;
using Workhours.Api.Models.Workweeks;
using Workhours.Domain;
using Workhours.Services;

namespace Workhours.Api.Controllers;

[ApiController]
[Route("api/weeks")]
[Authorize]
public sealed class WeeksController(IWeekService weekService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(WorkweekResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkweekResponse>> GetWeekAsync([FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var workweek = await weekService.GetWeekByDateAsync(userId, date, cancellationToken);
        return Ok(MapResponse(workweek));
    }

    [HttpPatch("{workweekId:guid}/finalize")]
    [ProducesResponseType(typeof(WorkweekResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkweekResponse>> UpdateFinalizedStateAsync(
        Guid workweekId,
        [FromBody] FinalizeWorkweekRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetRequiredUserId();
        var workweek = await weekService.SetFinalizedStateAsync(userId, workweekId, request.IsFinalized, cancellationToken);
        return Ok(MapResponse(workweek));
    }

    private static WorkweekResponse MapResponse(Workweek workweek)
    {
        return new WorkweekResponse(
            workweek.Id,
            workweek.WeekStart,
            workweek.WeekEnd,
            workweek.IsFinalized,
            workweek.CreatedAtUtc,
            workweek.UpdatedAtUtc);
    }
}