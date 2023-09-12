using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api.Features.Events.GetEvents;

[ApiController]
[Route("api/events")]
public sealed class GetEventsController : ControllerBase
{
    private readonly ISender _sender;

    public GetEventsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] EventParameters eventParams,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetEvents.Query { EventParameters = eventParams }
            , cancellationToken);
        
        Response.Headers.Add("Pagination", JsonSerializer.Serialize(response.Metadata));

        return Ok(response.EventsResponse);
    }
}