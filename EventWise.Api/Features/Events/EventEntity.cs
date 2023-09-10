﻿namespace EventWise.Api.Features.Events;

public sealed class EventEntity
{
    public required int EventId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public List<EventParticipant> Participants { get; set; } = new();
}

public sealed class EventParticipant
{
    public required int AppUserId { get; set; }
    public required AppUser AppUser { get; set; }
    public required int EventId { get; set; }
    public required EventEntity Event { get; set; }
    public required bool IsHost { get; set; }
}