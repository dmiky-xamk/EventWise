using EventWise.Api.Features.Events.CreateEvent;
using FluentValidation.TestHelper;

namespace EventWise.Api.Tests.Features.Events;

public sealed class ValidationTests
{
    private CreateEventRequestValidator EventRequestValidator { get; } = new();
    
    [Fact]
    public void FieldsShouldNotBeEmpty()
    {
        var request = new CreateEventRequest(" ", " ", " ", " ", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        
        var result = EventRequestValidator.TestValidate(request);
        
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.Address);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }
    
    [Fact]
    public void StartDateShouldBeLessThanEndDate()
    {
        var request = new CreateEventRequest("Name", "Description", "Address", "City", DateTime.UtcNow.AddDays(1), DateTime.UtcNow);
        
        var result = EventRequestValidator.TestValidate(request);
        
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }
    
    [Fact]
    public void StartDateShouldBeGreaterThanNow()
    {
        var request = new CreateEventRequest("Name", "Description", "Address", "City", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
        
        var result = EventRequestValidator.TestValidate(request);
        
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }
    
    [Fact]
    public void EndDateShouldBeGreaterThanNow()
    {
        var request = new CreateEventRequest("Name", "Description", "Address", "City", DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));
        
        var result = EventRequestValidator.TestValidate(request);
        
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}