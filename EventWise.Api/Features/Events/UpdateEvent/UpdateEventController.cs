using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api.Features.Events.UpdateEvent;

public sealed class UpdateEventController : EventControllerBase
{
    private readonly ISender _sender;

    public UpdateEventController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpPut("api/events/{publicId}")]
    public async Task<IActionResult> UpdateEvent(string publicId, [FromBody] UpdateEventRequest request)
    {
        var result = await _sender.Send(new UpdateEvent.Command {Event = request, PublicId = publicId });

        return result.Match<IActionResult>(
            response => Ok(response),
            validationError => ValidationProblem(CreateModelStateErrorsFrom(validationError)),
            _ => NotFound());
    }
}