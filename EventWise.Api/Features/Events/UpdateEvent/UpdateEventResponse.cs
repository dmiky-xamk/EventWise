using EventWise.Api.Features.Events.Domain;

namespace EventWise.Api.Features.Events.UpdateEvent;

public sealed record UpdateEventResponse(UpdateEventDto Event);

public sealed record UpdateEventDto(string Name, string Description, DateTime StartDate,
    DateTime EndDate, string City, string Address, IEnumerable<UpdateEventParticipantDto> Participants);

public sealed record UpdateEventParticipantDto(string Username);