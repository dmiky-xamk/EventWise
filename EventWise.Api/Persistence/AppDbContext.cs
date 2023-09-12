using EventWise.Api.Features.Events.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{
    // For unit testing
    public AppDbContext()
    { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public virtual DbSet<EventEntity> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventEntity>()
            .HasKey(x => x.EventId);

        modelBuilder.Entity<EventParticipantEntity>()
            .HasKey(x => new { x.AppUserId, x.EventId });
        
        modelBuilder.Entity<EventEntity>()
            .Property(x => x.PublicId)
            .HasValueGenerator<PublicIdValueGenerator>();
        
        modelBuilder.Entity<EventEntity>()
            .HasIndex(x => x.PublicId)
            .IsUnique();
    }
}