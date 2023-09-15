using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EventWise.Api;

public sealed class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandler(ILogger<ExceptionHandler> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception has occurred while executing the request");

        var problemDetails = _environment.IsDevelopment()
            ? CreateDevelopmentProblemDetails(exception, httpContext)
            : CreateProductionProblemDetails(exception, httpContext);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private ProblemDetails CreateDevelopmentProblemDetails(Exception exception, HttpContext httpContext)
    {
        return new ProblemDetails
        {
            Title = "An unexpected error occurred while processing your request.",
            Type = exception.GetType().Name,
            Status = httpContext.Response.StatusCode,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Detail = $"{exception.Message} {exception.StackTrace}",
        };
    }

    private ProblemDetails CreateProductionProblemDetails(Exception exception, HttpContext httpContext)
    {
        return new ProblemDetails
        {
            Title = "An unexpected error occurred while processing your request.",
            Status = StatusCodes.Status500InternalServerError,
        };
    }
}