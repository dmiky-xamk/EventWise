using System.ComponentModel.DataAnnotations.Schema;

namespace EventWise.Api.Features.Events.Domain;

[Table("Event")]
public sealed class EventEntity
{
    public int EventId { get; set; }
    public string PublicId { get; set; } = default!;
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public bool IsCancelled { get; set; } = false;
    public required IEnumerable<EventParticipantEntity> Participants { get; init; } =
        new List<EventParticipantEntity>();
}