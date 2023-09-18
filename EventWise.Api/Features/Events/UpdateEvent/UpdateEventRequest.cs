using FluentValidation;

namespace EventWise.Api.Features.Events.UpdateEvent;

public sealed record UpdateEventRequest(string Name, string Description, string Address, string City,
    DateTime StartDate,
    DateTime EndDate);

public sealed class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
{
    public UpdateEventRequestValidator()
    {
        RuleFor(x => x.Name).EventName();
        RuleFor(x => x.Description).EventDescription();
        RuleFor(x => x.Address).EventAddress();
        RuleFor(x => x.City).EventCity();
        RuleFor(x => x.StartDate).EventStartDate(x => x.EndDate);
        RuleFor(x => x.EndDate).EventEndDate(x => x.StartDate);
    }
}