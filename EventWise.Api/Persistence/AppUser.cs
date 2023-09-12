using EventWise.Api.Features.Events.Domain;
using Microsoft.AspNetCore.Identity;

namespace EventWise.Api.Persistence;

public sealed class AppUser : IdentityUser
{
    public List<EventParticipantEntity> Events { get; set; } = new();
}