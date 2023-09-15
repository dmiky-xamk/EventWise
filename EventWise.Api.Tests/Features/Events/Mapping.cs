using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Features.Events.GetEvent;
using EventWise.Api.Features.Events.GetEvents;

namespace EventWise.Api.Tests.Features.Events;

public static class Mapping
{
    public static GetEventDto ToEventDto(this EventEntity eventEntity)
        => new(
            eventEntity.Name,
            eventEntity.Description,
            eventEntity.Participants.First(x => x.IsHost).AppUser.UserName!,
            eventEntity.Address,
            eventEntity.City,
            eventEntity.StartDate,
            eventEntity.EndDate,
            eventEntity.Participants.Select(x => new GetEventParticipantDto(x.AppUser.UserName!)));
    
    public static GetEventsDto ToEventsDto(this EventEntity eventEntity)
        => new(
            eventEntity.PublicId,
            eventEntity.Name,
            eventEntity.Description,
            eventEntity.Participants.First(x => x.IsHost).AppUser.UserName!,
            eventEntity.Address,
            eventEntity.City,
            eventEntity.StartDate,
            eventEntity.EndDate,
            eventEntity.Participants.Select(x => new GetEventsParticipantDto(x.AppUser.UserName!)));

    public static GetEventsResponse ToEventsResponse(this IEnumerable<GetEventsDto> events)
        => new(events);
    
    public static GetEventResponse ToEventResponse(this GetEventDto eventDto)
        => new(eventDto);
}