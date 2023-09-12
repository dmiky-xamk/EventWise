using EventWise.Api.Features.Events.Domain;
using Microsoft.AspNetCore.Identity;

namespace EventWise.Api;

public sealed class AppUser : IdentityUser
{
    public List<EventParticipantEntity> Events { get; set; } = new();
}