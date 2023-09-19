using System.Security.Claims;
using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace EventWise.Api.Features.Events.UpdateParticipation;

public static class UpdateParticipation
{
    public sealed class Command : IRequest<OneOf<Success, NotFound, Error<string>>>
    {
        public required string PublicId { get; init; }
    }

    public sealed class Handler : IRequestHandler<Command, OneOf<Success, NotFound, Error<string>>>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public Handler(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<OneOf<Success, NotFound, Error<string>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await FindUser();
            
            var eventEntity = await FindEvent(request.PublicId, cancellationToken);
            if (eventEntity is null)
            {
                return new NotFound();
            }
            
            var participation = FindParticipation(eventEntity, user);
            if (participation is null)
            {
                return await Participate(eventEntity, user, cancellationToken);
            }

            return participation.IsHost switch
            {
                false => await UnParticipate(eventEntity, participation, cancellationToken),
                true => new Error<string>("The host cannot un-participate from their own event")
            };
        }

        private async Task<OneOf<Success, NotFound, Error<string>>> UnParticipate(EventEntity eventEntity, EventParticipantEntity participation, CancellationToken cancellationToken)
        {
            eventEntity.Participants.Remove(participation);
            await _context.SaveChangesAsync(cancellationToken);
            
            return new Success();
        }

        private async Task<OneOf<Success, NotFound, Error<string>>> Participate(EventEntity eventEntity, AppUser user, CancellationToken cancellationToken)
        {
            var participation = new EventParticipantEntity
            {
                Event = eventEntity,
                AppUser = user,
                IsHost = false
            };
            
            eventEntity.Participants.Add(participation);
            await _context.SaveChangesAsync(cancellationToken);
            
            return new Success();
        }

        private EventParticipantEntity? FindParticipation(EventEntity eventEntity, AppUser user)
        {
            return eventEntity.Participants.FirstOrDefault(p => p!.AppUserId == user.Id);
        }

        private async Task<AppUser> FindUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return (await _userManager.FindByIdAsync(userId))!;
        }

        private async Task<EventEntity?> FindEvent(string publicId, CancellationToken cancellationToken)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(ev => ev.Participants)
                .Where(ev => !ev.IsCancelled)
                .FirstOrDefaultAsync(ev => ev.PublicId == publicId, cancellationToken);
        }
    }
}