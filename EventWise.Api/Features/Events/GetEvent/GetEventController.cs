using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api.Features.Events.GetEvent;

[ApiController]
[Route("api/events")]
public class GetEventController : ControllerBase
{
    private readonly ISender _sender;

    public GetEventController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{publicId}")]
    public async Task<ActionResult<GetEventResponse>> GetEvent(string publicId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEvent.Query(publicId), cancellationToken);

        return result.Match<ActionResult<GetEventResponse>>(
            eventResponse => eventResponse,
            _ => NotFound());
    }
}