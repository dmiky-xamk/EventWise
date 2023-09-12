using System.ComponentModel.DataAnnotations.Schema;

namespace EventWise.Api.Features.Events.Domain;

[Table("EventParticipant")]
public sealed class EventParticipantEntity
{
    public required string AppUserId { get; set; }
    public required AppUser AppUser { get; set; }
    public required int EventId { get; set; }
    public required EventEntity Event { get; set; }
    public required bool IsHost { get; set; }
}