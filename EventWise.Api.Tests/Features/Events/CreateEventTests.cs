using AutoFixture;
using EventWise.Api.Features.Events.CreateEvent;
using EventWise.Api.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace EventWise.Api.Tests.Features.Events;

public sealed class CreateEventTests : IClassFixture<TestDatabaseFixture>
{
    private readonly UserManager<AppUser> _mockUserManager;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly Fixture _fixture = new();
    private TestDatabaseFixture TestDbFixture { get; }
    private readonly CreateEventRequestValidator _validator = new();

    public CreateEventTests(TestDatabaseFixture testDbFixture)
    {
        TestDbFixture = testDbFixture;
        _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _mockUserManager = Substitute.For<UserManager<AppUser>>(
            Substitute.For<IUserStore<AppUser>>(),
            Substitute.For<IOptions<IdentityOptions>>(),
            Substitute.For<IPasswordHasher<AppUser>>(), 
            Array.Empty<IUserValidator<AppUser>>(),
            Array.Empty<IPasswordValidator<AppUser>>(),
            Substitute.For<ILookupNormalizer>(),
            Substitute.For<IdentityErrorDescriber>(),
            Substitute.For<IServiceProvider>(),
            Substitute.For<ILogger<UserManager<AppUser>>>());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateEvent_CreatesAndReturnsEvent_WhenSuccessful()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var sut = new CreateEvent.Handler(context, _mockHttpContextAccessor, _mockUserManager, _validator);

        var expected = _fixture.Build<CreateEventRequest>()
            .With(x => x.StartDate, DateTime.Now.AddDays(1))
            .With(x => x.EndDate, DateTime.Now.AddDays(2))
            .Create();
        var user = _fixture.Create<AppUser>();
        _mockUserManager.FindByIdAsync(Arg.Any<string>()).Returns(user);

        // Act
        var actual = await sut.Handle(new CreateEvent.Command { Event = expected }, default);
        context.ChangeTracker.Clear();

        // Assert
        actual.Value.As<CreateEventResponse>().Event.Should().BeEquivalentTo(expected);
        actual.Value.As<CreateEventResponse>().Event.Participants.Should()
            .ContainSingle(x => x.Username == user.UserName);
        actual.Value.As<CreateEventResponse>().Event.PublicId.Should().NotBeNull();
    }
}

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

    private AppDbContext CreateContext()
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