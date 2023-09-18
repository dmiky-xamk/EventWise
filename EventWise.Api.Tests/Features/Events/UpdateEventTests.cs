using AutoFixture;
using EventWise.Api.Features.Events.UpdateEvent;
using EventWise.Api.Persistence;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using OneOf.Types;

namespace EventWise.Api.Tests.Features.Events;

public sealed class UpdateEventTests : IClassFixture<TestDatabaseFixture>
{
    private readonly UserManager<AppUser> _mockUserManager;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly Fixture _fixture = new();
    private TestDatabaseFixture TestDbFixture { get; }
    private readonly UpdateEventRequestValidator _validator = new();

    public UpdateEventTests(TestDatabaseFixture testDbFixture)
    {
        TestDbFixture = testDbFixture;
        _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _mockUserManager = TestHelpers.MockUserManager();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task UpdateEvent_UpdatesAndReturnsEvent_WhenSuccessful()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);
        var eventEntity = TestHelpers.CreateEventAsHost(_fixture, context, user);
        var expected = _fixture.Build<UpdateEventRequest>()
            .With(x => x.StartDate, DateTime.Now.AddDays(1))
            .With(x => x.EndDate, DateTime.Now.AddDays(2))
            .Create();

        var sut = new UpdateEvent.Handler(context, _mockHttpContextAccessor, _mockUserManager, _validator);

        // Act
        var actual = await sut.Handle(new UpdateEvent.Command { Event = expected, PublicId = eventEntity.PublicId },
            default);
        context.ChangeTracker.Clear();

        // Assert
        actual.Value.As<UpdateEventResponse>().Event.Should().BeEquivalentTo(expected);
        actual.Value.As<UpdateEventResponse>().Event.Participants.Should()
            .ContainSingle(x => x.Username == user.UserName);
    }

    [Fact]
    public async Task UpdateEvent_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContext();
        TestHelpers.CreateUser(_fixture, _mockUserManager);
        var expected = _fixture.Build<UpdateEventRequest>()
            .With(x => x.StartDate, DateTime.Now.AddDays(1))
            .With(x => x.EndDate, DateTime.Now.AddDays(2))
            .Create();

        var sut = new UpdateEvent.Handler(context, _mockHttpContextAccessor, _mockUserManager, _validator);

        // Act
        var actual =
            await sut.Handle(new UpdateEvent.Command { Event = expected, PublicId = _fixture.Create<string>() },
                default);

        // Assert
        actual.Value.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task UpdateEvent_ReturnsNotFound_WhenUserIsNotHost()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContext();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);
        var eventEntity = TestHelpers.CreateEventAsParticipant(_fixture, context, user);
        var request = _fixture.Build<UpdateEventRequest>()
            .With(x => x.StartDate, DateTime.Now.AddDays(1))
            .With(x => x.EndDate, DateTime.Now.AddDays(2))
            .Create();

        var sut = new UpdateEvent.Handler(context, _mockHttpContextAccessor, _mockUserManager, _validator);

        // Act
        var actual = await sut.Handle(new UpdateEvent.Command { Event = request, PublicId = eventEntity.PublicId },
            default);

        // Assert
        actual.Value.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task UpdateEvent_ReturnsError_WhenValidationFails()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContext();
        TestHelpers.CreateUser(_fixture, _mockUserManager);
        var request = _fixture.Build<UpdateEventRequest>()
            .With(x => x.Name, string.Empty)
            .Create();

        var sut = new UpdateEvent.Handler(context, _mockHttpContextAccessor, _mockUserManager, _validator);

        // Act
        var actual = await sut.Handle(new UpdateEvent.Command { Event = request, PublicId = _fixture.Create<string>() },
            default);

        // Assert
        actual.Value.Should().BeOfType<ValidationResult>();
    }
}