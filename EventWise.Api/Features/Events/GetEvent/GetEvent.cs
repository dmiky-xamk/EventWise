using EventWise.Api.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace EventWise.Api.Features.Events.GetEvent;

public static class GetEvent
{
    public sealed class Query : IRequest<OneOf<GetEventResponse, NotFound>>
    {
        public string PublicId { get; }

        public Query(string publicId)
        {
            PublicId = publicId;
        }
    }

    public sealed class Handler : IRequestHandler<Query, OneOf<GetEventResponse, NotFound>>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<OneOf<GetEventResponse, NotFound>> Handle(Query query, CancellationToken cancellationToken)
        {
            var eventDto = await _context.Events
                .AsNoTracking()
                .Where(ev => ev.PublicId == query.PublicId)
                .Select(ev => new GetEventDto(
                    ev.Name,
                    ev.Description,
                    ev.Participants.First(x => x.IsHost).AppUser.UserName!,
                    ev.Address,
                    ev.City,
                    ev.StartDate,
                    ev.EndDate,
                    ev.Participants.Select(x => new GetEventParticipantDto(x.AppUser.UserName!))))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return eventDto is null 
                ? new NotFound() 
                : new GetEventResponse(eventDto);
        }
    }
}