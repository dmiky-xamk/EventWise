using EventWise.Api.Features.Events.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Persistence;

public static class DbInitializer
{
    public static IApplicationBuilder InitializeDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        
        SeedDb(context, userManager);
        
        return app;
    }

    private static void SeedDb(AppDbContext context, UserManager<AppUser> userManager)
    {
        context.Database.Migrate();
        
        if (context.Events.Any() || context.Users.Any())
            return;
        
        var users = new[]
        {
            new AppUser
            {
                UserName = "user1",
                Email = "user1@eventwise.com"
            },
            new AppUser
            {
                UserName = "user2",
                Email = "user2@eventwise.com"
            }
        };

        foreach (var user in users)
        {
            userManager.CreateAsync(user, "Testi1").Wait();
        }
        
        var events = new List<EventEntity>
        {
            new()
            {
                Name = "Event 1",
                Description = "Event 1 Description",
                Address = "Event 1 Address",
                City = "Event 1 City",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Participants = new List<EventParticipantEntity>
                {
                    new()
                    {
                        AppUser = users[0],
                        IsHost = true
                    },
                    new()
                    {
                        AppUser = users[1],
                        IsHost = false
                    }
                }
            },
            new()
            {
                Name = "Event 2",
                Description = "Event 2 Description",
                Address = "Event 2 Address",
                City = "Event 2 City",
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(8),
                Participants = new List<EventParticipantEntity>
                {
                    new()
                    {
                        AppUser = users[1],
                        IsHost = true
                    },
                    new()
                    {
                        AppUser = users[0],
                        IsHost = false
                    }
                }
                
            }
        };

        context.Events.AddRange(events);
        context.SaveChanges();
    }
}