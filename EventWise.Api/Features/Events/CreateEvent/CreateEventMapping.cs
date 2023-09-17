using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Persistence;

namespace EventWise.Api.Features.Events.CreateEvent;

public static class CreateEventMapping
{
    public static EventEntity MapToEventEntityWith(this CreateEventRequest request, AppUser user)
    {
        return new EventEntity
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Participants = new List<EventParticipantEntity>
            {
                new()
                {
                    AppUser = user,
                    IsHost = true,
                }
            }
        };
    }

    public static CreatedEventDto MapToCreatedEventDto(this EventEntity createdEvent)
    {
        return new CreatedEventDto(
            createdEvent.PublicId,
            createdEvent.Name,
            createdEvent.Description,
            createdEvent.Address,
            createdEvent.City,
            createdEvent.StartDate,
            createdEvent.EndDate,
            createdEvent.Participants.Select(x => new CreateEventParticipantDto(x.AppUser.UserName!))
        );
    }
}