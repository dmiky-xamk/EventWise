namespace EventWise.Api.Features.Events.CreateEvent;

public record CreateEventResponse(CreatedEventDto Event);

public record CreatedEventDto(string PublicId, string Name, string Description, string Address, string City,
    DateTime StartDate,
    DateTime EndDate, IEnumerable<CreateEventParticipantDto> Participants);

public record CreateEventParticipantDto(string Username);