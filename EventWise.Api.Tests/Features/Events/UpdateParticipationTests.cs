﻿using AutoFixture;
using EventWise.Api.Features.Events.UpdateParticipation;
using EventWise.Api.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using OneOf.Types;

namespace EventWise.Api.Tests.Features.Events;

public sealed class UpdateParticipationTests : IClassFixture<TestDatabaseFixture>
{
    private readonly UserManager<AppUser> _mockUserManager;
    private readonly IHttpContextAccessor _mockHttpContextAccessor;
    private readonly Fixture _fixture = new();
    private TestDatabaseFixture TestDbFixture { get; }

    public UpdateParticipationTests(TestDatabaseFixture testDbFixture)
    {
        TestDbFixture = testDbFixture;
        _mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _mockUserManager = TestHelpers.MockUserManager();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task UserCanParticipateInEvent()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);
        var eventEntity = TestHelpers.CreateEventAsParticipant(_fixture, context, user);

        var sut = new UpdateParticipation.Handler(context, _mockHttpContextAccessor, _mockUserManager);

        // Act
        var actual = await sut.Handle(new UpdateParticipation.Command { PublicId = eventEntity.PublicId }, default);
        context.ChangeTracker.Clear();
     
        // // Assert
        actual.Value.Should().BeOfType<Success>();
    }
    
    [Fact]
    public async Task UserCannotParticipateInEventTwice()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);
        var eventEntity = TestHelpers.CreateEventAsParticipant(_fixture, context, user);

        var sut = new UpdateParticipation.Handler(context, _mockHttpContextAccessor, _mockUserManager);

        // Act
        var actual = await sut.Handle(new UpdateParticipation.Command { PublicId = eventEntity.PublicId }, default);
        context.ChangeTracker.Clear();
     
        // Assert
        actual.Value.Should().BeOfType<Success>();
    }
    
    [Fact]
    public async Task UserCannotParticipateInNonExistingEvent()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);

        var sut = new UpdateParticipation.Handler(context, _mockHttpContextAccessor, _mockUserManager);

        // Act
        var actual = await sut.Handle(new UpdateParticipation.Command { PublicId = "non-existing-event" }, default);
        context.ChangeTracker.Clear();
     
        // // Assert
        actual.Value.Should().BeOfType<NotFound>();
    }
    
    [Fact]
    public async Task UserCannotParticipateInCancelledEvent()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);
        var eventEntity = TestHelpers.CreateEventAsParticipant(_fixture, context, user);
        eventEntity.IsCancelled = true;
        await context.SaveChangesAsync();

        var sut = new UpdateParticipation.Handler(context, _mockHttpContextAccessor, _mockUserManager);

        // Act
        var actual = await sut.Handle(new UpdateParticipation.Command { PublicId = eventEntity.PublicId }, default);
        context.ChangeTracker.Clear();
     
        // // Assert
        actual.Value.Should().BeOfType<NotFound>();
    }
    
    [Fact]
    public async Task HostCannotDropParticipation()
    {
        // Arrange
        await using var context = TestDbFixture.CreateContextWithTransaction();
        var user = TestHelpers.CreateUser(_fixture, _mockUserManager);
        var eventEntity = TestHelpers.CreateEventAsHost(_fixture, context, user);

        var sut = new UpdateParticipation.Handler(context, _mockHttpContextAccessor, _mockUserManager);

        // Act
        var actual = await sut.Handle(new UpdateParticipation.Command { PublicId = eventEntity.PublicId }, default);
        context.ChangeTracker.Clear();
     
        // // Assert
        actual.Value.Should().BeOfType<Error<string>>();
    }
}