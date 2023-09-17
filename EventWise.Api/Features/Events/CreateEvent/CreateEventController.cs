using EventWise.Api.Features.Events.GetEvent;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EventWise.Api.Features.Events.CreateEvent;

public sealed class CreateEventController : ControllerBase
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
    
    private ModelStateDictionary CreateModelStateErrorsFrom(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ModelState;
    }

    private CreatedAtActionResult CreatedAtActionResponse(CreateEventResponse createdEventResponse)
    {
        var actionName = nameof(GetEventController.GetEvent);
        
        return CreatedAtAction(actionName, new { publicId = createdEventResponse.Event.PublicId }, createdEventResponse);
    }
}