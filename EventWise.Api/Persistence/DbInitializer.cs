using EventWise.Api.Features.Events.Domain;

namespace EventWise.Api.Persistence;

public static class DbInitializer
{
    public static IApplicationBuilder InitializeDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        SeedDb(context);
        
        return app;
    }

    private static void SeedDb(AppDbContext context)
    {
        context.Database.EnsureCreated();
        
        if (context.Events.Any())
            return;
        
        var users = new[]
        {
            new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user1",
                Email = "user1@eventwise.com"
            },
            new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "user2",
                Email = "user2@eventwise.com"
            }
        };

        context.Users.AddRange(users);
        
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