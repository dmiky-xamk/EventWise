using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventWise.Api.Features.Events.GetEvents;

public static class GetEvents
{
    public sealed class Query : IRequest<PagedResponse>
    {
        public required EventParameters EventParameters { get; init; }
    }

    public sealed class Handler : IRequestHandler<Query, PagedResponse>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<PagedResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<EventEntity> eventsQuery = _context.Events;

            eventsQuery = FilterByEventName(request, eventsQuery);
            eventsQuery = FilterByEventDates(request, eventsQuery);
            eventsQuery = SortEvents(request, eventsQuery);

            var events = await eventsQuery
                .AsNoTracking()
                .Skip((request.EventParameters.Page - 1) * request.EventParameters.PageSize)
                .Take(request.EventParameters.PageSize)
                .Select(ev => new GetEventsDto(
                    ev.PublicId,
                    ev.Name,
                    ev.Description,
                    ev.Participants.First(x => x.IsHost).AppUser.UserName!,
                    ev.Address,
                    ev.City,
                    ev.StartDate,
                    ev.EndDate,
                    ev.Participants.Select(x => new GetEventsParticipantDto(x.AppUser.UserName!))))
                .ToListAsync(cancellationToken);

            var eventsCount = await eventsQuery.CountAsync(cancellationToken);

            var metadata = new GetEventsMetadata(
                eventsCount,
                request.EventParameters.PageSize,
                request.EventParameters.Page,
                (int)Math.Ceiling(eventsCount / (double)request.EventParameters.PageSize));

            return new PagedResponse(new GetEventsResponse(events), metadata);
        }

        private static IQueryable<EventEntity> SortEvents(Query request, IQueryable<EventEntity> eventsQuery)
        {
            var orderByQueryString = request.EventParameters.OrderBy.Split('_');
            var sortField = orderByQueryString[0];
            var sortOrder = orderByQueryString.Length > 1 ? orderByQueryString[1] : string.Empty;

            eventsQuery = sortField switch
            {
                "startDate" => sortOrder switch
                {
                    "desc" => eventsQuery.OrderByDescending(e => e.StartDate),
                    _ => eventsQuery.OrderBy(e => e.StartDate)
                },
                "name" => sortOrder switch
                {
                    "desc" => eventsQuery.OrderByDescending(e => e.Name),
                    _ => eventsQuery.OrderBy(e => e.Name)
                },
                _ => eventsQuery.OrderBy(e => e.StartDate)
            };
            return eventsQuery;
        }

        private static IQueryable<EventEntity> FilterByEventDates(Query request, IQueryable<EventEntity> eventsQuery)
        {
            eventsQuery = eventsQuery.Where(e => e.StartDate >= request.EventParameters.StartDate
                                                 && e.EndDate <= request.EventParameters.EndDate);
            return eventsQuery;
        }

        private static IQueryable<EventEntity> FilterByEventName(Query request, IQueryable<EventEntity> eventsQuery)
        {
            if (!string.IsNullOrWhiteSpace(request.EventParameters.EventName))
            {
                // TODO: Test with integration tests. Unit test doesn't work with EF.Functions.Like.
                eventsQuery = eventsQuery.Where(e => EF.Functions.Like(e.Name, $"%{request.EventParameters.EventName}%"));
            }

            return eventsQuery;
        }
    }
}