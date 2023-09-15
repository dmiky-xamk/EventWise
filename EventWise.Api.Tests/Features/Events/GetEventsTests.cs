using AutoFixture;
using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Features.Events.GetEvents;
using EventWise.Api.Persistence;
using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace EventWise.Api.Tests.Features.Events;

public sealed class GetEventsTests
{
    private readonly GetEvents.Handler _sut;
    private readonly AppDbContext _mockContext;
    private readonly Fixture _fixture = new();

    public GetEventsTests()
    {
        _mockContext = Substitute.For<AppDbContext>();
        _sut = new GetEvents.Handler(_mockContext);
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetEvents_ShouldReturnEmptyList_WhenNoEventsExist()
    {
        // Arrange
        var events = Enumerable.Empty<EventEntity>().AsQueryable().BuildMockDbSet();
        _mockContext.Events.Returns(events);

        // Act
        var result = await _sut.Handle(new GetEvents.Query { EventParameters = new EventParameters() }, default);

        // Assert
        result.EventsResponse.Events.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEvents_ShouldReturnEvents_WhenEventsExist()
    {
        // Arrange
        var events = _fixture.CreateMany<EventEntity>().AsQueryable().BuildMockDbSet();
        var expected = events.Select(ev => ev.ToEventsDto()).ToEventsResponse();
        _mockContext.Events.Returns(events);

        // Act
        var actual = await _sut.Handle(new GetEvents.Query { EventParameters = new EventParameters() }, default);

        // Assert
        actual.EventsResponse.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetEvents_ShouldReturnEvents_WhenEventsExistAndPageIsGreaterThanOne()
    {
        // Arrange
        const int totalEvents = 10;
        const int currentPage = 2;
        const int pageSize = 5;
        var events = _fixture.CreateMany<EventEntity>(totalEvents).AsQueryable().BuildMockDbSet();
        var expected = events.OrderBy(ev => ev.StartDate).Skip(pageSize).Select(ev => ev.ToEventsDto())
            .ToEventsResponse();
        var eventParameters = new EventParameters { Page = currentPage, PageSize = pageSize };
        _mockContext.Events.Returns(events);

        // Act
        var actual = await _sut.Handle(new GetEvents.Query { EventParameters = eventParameters }, default);

        // Assert
        actual.EventsResponse.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetEvents_PageCantHaveMoreResults_ThanMaxPageSize()
    {
        // Arrange
        const int maxPageSize = 20; // This is dependant on the EventParameters class' MaxPageSize constant
        const int pageSize = 30;
        var events = _fixture.CreateMany<EventEntity>(pageSize).AsQueryable().BuildMockDbSet();
        var eventParameters = new EventParameters { Page = 1, PageSize = pageSize };
        _mockContext.Events.Returns(events);

        // Act
        var actual = await _sut.Handle(new GetEvents.Query { EventParameters = eventParameters },
            default);

        // Assert
        actual.Metadata.PageSize.Should().Be(maxPageSize);
        actual.EventsResponse.Events.Should().HaveCount(maxPageSize);
    }

    [Fact]
    public async Task GetEvents_ShouldBeFilterable_WithEventDates()
    {
        // Arrange
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var todaysEvent = _fixture.Build<EventEntity>()
            .With(ev => ev.StartDate, today)
            .With(ev => ev.EndDate, tomorrow)
            .Create();
        var tomorrowsEvent = _fixture.Build<EventEntity>()
            .With(ev => ev.StartDate, tomorrow)
            .With(ev => ev.EndDate, tomorrow.AddDays(1))
            .Create();
        var events = new List<EventEntity> { todaysEvent, tomorrowsEvent }.AsQueryable().BuildMockDbSet();
        var eventParameters = new EventParameters { StartDate = today, EndDate = tomorrow };
        var expected = new GetEventsResponse(new List<GetEventsDto> { todaysEvent.ToEventsDto() });
        _mockContext.Events.Returns(events);

        // Act
        var actual = await _sut.Handle(new GetEvents.Query { EventParameters = eventParameters },
            default);

        // Assert
        actual.EventsResponse.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetEvents_ShouldBeSortableAscending()
    {
        var events = _fixture.CreateMany<EventEntity>(5).AsQueryable().BuildMockDbSet();
        var eventParameters = new EventParameters { OrderBy = "name" };
        var expected = events.OrderBy(ev => ev.Name).Select(ev => ev.ToEventsDto()).ToEventsResponse();
        _mockContext.Events.Returns(events);

        // Act
        var actual = await _sut.Handle(new GetEvents.Query { EventParameters = eventParameters },
            default);

        // Assert
        actual.EventsResponse.Should().BeEquivalentTo(expected,
            opt => opt.WithStrictOrdering());
    }
    
    [Fact]
    public async Task GetEvents_ShouldBeSortableDescending()
    {
        var events = _fixture.CreateMany<EventEntity>(5).AsQueryable().BuildMockDbSet();
        var eventParameters = new EventParameters { OrderBy = "name_desc" };
        var expected = events.OrderByDescending(ev => ev.Name).Select(ev => ev.ToEventsDto()).ToEventsResponse();
        _mockContext.Events.Returns(events);

        // Act
        var actual = await _sut.Handle(new GetEvents.Query { EventParameters = eventParameters },
            default);

        // Assert
        actual.EventsResponse.Should().BeEquivalentTo(expected,
            opt => opt.WithStrictOrdering());
    }
}