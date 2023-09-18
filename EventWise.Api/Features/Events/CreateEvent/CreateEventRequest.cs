using FluentValidation;

namespace EventWise.Api.Features.Events.CreateEvent;

public record CreateEventRequest(string Name, string Description, string Address, string City, DateTime StartDate,
    DateTime EndDate);

public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.Name).EventName();
        RuleFor(x => x.Description).EventDescription();
        RuleFor(x => x.Address).EventAddress();
        RuleFor(x => x.City).EventCity();
        RuleFor(x => x.StartDate).EventStartDate(x => x.EndDate);
        RuleFor(x => x.EndDate).EventEndDate(x => x.StartDate);
    }
}