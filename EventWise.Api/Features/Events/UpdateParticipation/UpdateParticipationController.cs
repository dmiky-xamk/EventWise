using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api.Features.Events.UpdateParticipation;

public sealed class UpdateParticipationController : ControllerBase
{
    private readonly ISender _sender;

    public UpdateParticipationController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpPut("api/events/{publicId}/participate")]
    public async Task<IActionResult> Participate(string publicId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateParticipation.Command { PublicId = publicId }, cancellationToken);

        return result.Match<IActionResult>(
            success => Ok(),
            notFound => NotFound(),
            error => BadRequest(new ProblemDetails
            {
                Title = "An error occurred while updating participation",
                Detail = error.Value,
                Status = StatusCodes.Status400BadRequest
            }));
    }
}