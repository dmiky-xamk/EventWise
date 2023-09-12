using AutoFixture;
using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Features.Events.GetEvent;
using EventWise.Api.Persistence;
using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using OneOf.Types;

namespace EventWise.Api.Tests.Features.Events;

public sealed class GetEventTests
{
    private readonly GetEvent.Handler _sut;
    private readonly AppDbContext _mockContext;
    private readonly Fixture _fixture = new();

    public GetEventTests()
    {
        _mockContext = Substitute.For<AppDbContext>();
        _sut = new GetEvent.Handler(_mockContext);
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Fact]
    public async Task GetEvent_ShouldReturnNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        var url = _fixture.Create<string>();
        var events = Enumerable.Empty<EventEntity>().AsQueryable().BuildMockDbSet();
        _mockContext.Events.Returns(events);
        
        // Act
        var result = await _sut.Handle(new GetEvent.Query(url), default);

        // Assert
        result.Value.Should().BeOfType<NotFound>();
    }
    
    [Fact]
    public async Task GetEvent_ShouldReturnEvent_WhenEventExists()
    {
        // Arrange
        var events = _fixture.Build<EventEntity>()
            .CreateMany()
            .AsQueryable()
            .BuildMockDbSet();
        var url = events.First().PublicId;
        var expected = events.First().ToEventDto().ToEventResponse();
        _mockContext.Events.Returns(events);
        
        // Act
        var actual = await _sut.Handle(new GetEvent.Query(url), default);
        
        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }
}