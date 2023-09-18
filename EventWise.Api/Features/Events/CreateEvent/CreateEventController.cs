using EventWise.Api.Features.Events.GetEvent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api.Features.Events.CreateEvent;

public sealed class CreateEventController : EventControllerBase
{
    private readonly ISender _sender;

    public CreateEventController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpPost("api/events")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new CreateEvent.Command { Event = request }, cancellationToken);
        
        return response.Match<IActionResult>(
            CreatedAtActionResponse,
            validationResult => ValidationProblem(CreateModelStateErrorsFrom(validationResult)));
    }

    private CreatedAtActionResult CreatedAtActionResponse(CreateEventResponse createdEventResponse)
    {
        var actionName = nameof(GetEventController.GetEvent);
        var controllerName = nameof(GetEventController).Replace("Controller", "");
        
        return CreatedAtAction(actionName, controllerName, new { publicId = createdEventResponse.Event.PublicId }, createdEventResponse);
    }
}