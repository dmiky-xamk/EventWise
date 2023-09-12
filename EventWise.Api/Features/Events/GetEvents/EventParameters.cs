namespace EventWise.Api.Features.Events.GetEvents;

public sealed class EventParameters
{
    public string EventName { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public DateTime StartDate { get; init; } = DateTime.MinValue;
    public DateTime EndDate { get; init; } = DateTime.MaxValue;
    public string OrderBy { get; init; } = "startDate";

    private const int MaxPageSize = 20;
    private readonly int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}