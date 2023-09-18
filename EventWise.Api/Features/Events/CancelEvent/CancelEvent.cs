using System.Security.Claims;
using EventWise.Api.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace EventWise.Api.Features.Events.CancelEvent;

public static class CancelEvent
{
    public sealed class Command : IRequest<OneOf<Success, NotFound>>
    {
        public required string PublicId { get; init; }
    }

    public sealed class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
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

        public async Task<OneOf<Success, NotFound>> Handle(Command command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(
                _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var eventEntity = await _context.Events
                .Where(ev => ev.Participants.Any(p => p.AppUserId == user!.Id && p.IsHost) && !ev.IsCancelled)
                .FirstOrDefaultAsync(ev => ev.PublicId == command.PublicId, cancellationToken);

            if (eventEntity is null)
            {
                return new NotFound();
            }

            eventEntity.IsCancelled = true;
            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}