using EventWise.Api.Features.Events.Domain;

namespace EventWise.Api.Features.Events.UpdateEvent;

public static class UpdateEventMapping
{
    public static void UpdateWith(this EventEntity entity, UpdateEventRequest request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.City = request.City;
        entity.Address = request.Address;
    }
    
    public static UpdateEventDto MapToUpdateEventDto(this EventEntity entity)
    {
        return new UpdateEventDto(
            entity.Name,
            entity.Description,
            entity.StartDate,
            entity.EndDate,
            entity.City,
            entity.Address,
            entity.Participants.Select(x => new UpdateEventParticipantDto(x.AppUser.UserName!)));
    }
}