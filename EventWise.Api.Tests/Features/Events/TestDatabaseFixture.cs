using EventWise.Api.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventWise.Api.Tests.Features.Events;

public class TestDatabaseFixture
{
    private const string ConnectionString = "Data Source=EventWise.db";

    private static readonly object Lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (Lock)
        {
            if (_databaseInitialized) return;

            using var context = CreateContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            _databaseInitialized = true;
        }
    }
    
    public AppDbContext CreateContext()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(ConnectionString)
            .Options);

    public AppDbContext CreateContextWithTransaction()
    {
        var context = CreateContext();
        context.Database.BeginTransaction();

        return context;
    }
}