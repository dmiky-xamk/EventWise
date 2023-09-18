using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api.Features.Events.CancelEvent;

public sealed class CancelEventController : ControllerBase
{
    private readonly ISender _sender;

    public CancelEventController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpDelete("api/events/{publicId}")]
    public async Task<IActionResult> CancelEvent(string publicId)
    {
        var result = await _sender.Send(new CancelEvent.Command { PublicId = publicId });

        return result.Match<IActionResult>(
            success => Ok(),
            notFound => NotFound());
    }
}