using AutoFixture;
using EventWise.Api.Features.Events.CreateEvent;
using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Features.Events.UpdateEvent;
using EventWise.Api.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace EventWise.Api.Tests.Features.Events;

public static class TestHelpers
{
    public static UserManager<AppUser> MockUserManager()
    {
        return Substitute.For<UserManager<AppUser>>(
            Substitute.For<IUserStore<AppUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<IPasswordHasher<AppUser>>(),
            Array.Empty<IUserValidator<AppUser>>(),
            Array.Empty<IPasswordValidator<AppUser>>(),
            Substitute.For<ILookupNormalizer>(),
            Substitute.For<IdentityErrorDescriber>(),
            Substitute.For<IServiceProvider>(),
            Substitute.For<ILogger<UserManager<AppUser>>>());
    }

    public static AppUser CreateUser(IFixture fixture, UserManager<AppUser> userManager)
    {
        var user = fixture.Create<AppUser>();
        userManager.FindByIdAsync(Arg.Any<string>()).Returns(user);

        return user;
    }

    public static EventEntity CreateEventAsHost(this IFixture fixture, AppUser user)
        => fixture.CreateEventWith(user, true);
    
    public static EventEntity CreateEventAsParticipant(this IFixture fixture, AppUser user)
        => fixture.CreateEventWith(user, false);

    public static EventEntity CreateEventAsParticipant(IFixture fixture, AppDbContext context, AppUser user)
    {
        var eventEntity = fixture.CreateEventWith(user, false);
        context.Events.Add(eventEntity);
        context.SaveChanges();
        
        return eventEntity;
    }
    
    public static EventEntity CreateEventAsHost(IFixture fixture, AppDbContext context, AppUser user)
    {
        var eventEntity = fixture.CreateEventWith(user, true);
        context.Events.Add(eventEntity);
        context.SaveChanges();
        
        return eventEntity;
    }
    
    private static EventEntity CreateEventWith(this IFixture fixture, AppUser user, bool isHost)
        => fixture.Build<EventEntity>()
            .With(x => x.Participants, new List<EventParticipantEntity>
            {
                new() { AppUser = user, IsHost = isHost }
            })
            .With(x => x.IsCancelled, false)
            .Create(); 
    
    public static Task AddAndSaveEvent(this AppDbContext context, EventEntity eventEntity)
    {
        context.Events.Add(eventEntity);
        return context.SaveChangesAsync();
    }
}