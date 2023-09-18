using System.Linq.Expressions;
using FluentValidation;

namespace EventWise.Api.Features.Events;

public static class EventRequestValidator
{
    public static IRuleBuilderOptions<T, string> EventName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Event name is required and must not exceed 50 characters");
    }

    public static IRuleBuilderOptions<T, string> EventDescription<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Event description is required and must not exceed 100 characters");
    }

    public static IRuleBuilderOptions<T, string> EventAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Event address is required and must not exceed 100 characters");
    }

    public static IRuleBuilderOptions<T, string> EventCity<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("City name is required and must not exceed 50 characters");
    }

    public static IRuleBuilderOptions<T, DateTime> EventStartDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder,
        Expression<Func<T, DateTime>> endDateExpression)
    {
        return ruleBuilder
            .NotEmpty()
            .Must(BeAValidFutureDate).WithMessage("Start date must be a valid future date")
            .LessThan(endDateExpression).WithMessage("Start date must be before end date");
    }

    public static IRuleBuilderOptions<T, DateTime> EventEndDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder,
        Expression<Func<T, DateTime>> startDateExpression)
    {
        return ruleBuilder
            .NotEmpty()
            .Must(BeAValidFutureDate).WithMessage("End date must be a valid future date")
            .GreaterThan(startDateExpression).WithMessage("End date must be after start date");
    }

    private static bool BeAValidFutureDate(DateTime date)
    {
        return date >= DateTime.UtcNow;
    }
}