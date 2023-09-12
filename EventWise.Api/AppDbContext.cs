using EventWise.Api.Features.Events.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventWise.Api;

public sealed class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    
    public required DbSet<EventEntity> Events { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventEntity>()
            .HasKey(x => x.EventId);
        
        modelBuilder.Entity<EventParticipantEntity>()
            .HasKey(x => new { x.AppUserId, x.EventId });
    }
}