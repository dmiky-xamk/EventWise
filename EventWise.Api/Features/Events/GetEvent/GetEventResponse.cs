namespace EventWise.Api.Features.Events.GetEvent;

public sealed record GetEventResponse(GetEventDto Event);

public record GetEventDto(
    string Name,
    string Description,
    string HostUsername,
    string Address,
    string City,
    DateTime StartDate,
    DateTime EndDate,
    IEnumerable<GetEventParticipantDto> Participants);

public sealed record GetEventParticipantDto(string Username);