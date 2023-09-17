using FluentValidation;

namespace EventWise.Api.Features.Events.CreateEvent;

public record CreateEventRequest(string Name, string Description, string Address, string City, DateTime StartDate,
    DateTime EndDate);

public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Event name is required and must not exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Event description is required and must not exceed 100 characters");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Event address is required and must not exceed 100 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City name is required and must not exceed 50 characters");
        
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .Must(BeAValidFutureDate).WithMessage("Start date must be a valid future date")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .Must(BeAValidFutureDate).WithMessage("End date must be a valid future date")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date");
    }
    
    private static bool BeAValidFutureDate(DateTime date)
    {
        return date >= DateTime.UtcNow;
    }
}