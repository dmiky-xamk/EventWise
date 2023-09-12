namespace EventWise.Api.Features.Events.GetEvents;

public sealed record GetEventsResponse(IEnumerable<GetEventsDto> Events);

public sealed record PagedResponse(GetEventsResponse EventsResponse, GetEventsMetadata Metadata);

public sealed record GetEventsMetadata(int TotalCount, int PageSize, int CurrentPage, int TotalPages);

public sealed record GetEventsDto(
    string PublicId,
    string Name,
    string Description,
    string HostUsername,
    string Address,
    string City,
    DateTime StartDate,
    DateTime EndDate,
    IEnumerable<GetEventsParticipantDto> Participants);

public sealed record GetEventsParticipantDto(string Username);