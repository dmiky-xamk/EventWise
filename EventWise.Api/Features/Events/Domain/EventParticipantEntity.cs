using System.ComponentModel.DataAnnotations.Schema;
using EventWise.Api.Persistence;

namespace EventWise.Api.Features.Events.Domain;

[Table("EventParticipant")]
public sealed class EventParticipantEntity
{
    public string AppUserId { get; set; } = default!;
    public required AppUser AppUser { get; set; }
    public int EventId { get; set; } = default!;
    public EventEntity Event { get; set; } = default!;
    public required bool IsHost { get; set; }
}