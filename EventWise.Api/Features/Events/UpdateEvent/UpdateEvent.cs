using System.Security.Claims;
using EventWise.Api.Persistence;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace EventWise.Api.Features.Events.UpdateEvent;

public static class UpdateEvent
{
    public sealed class Command : IRequest<OneOf<UpdateEventResponse, ValidationResult, NotFound>>
    {
        public required UpdateEventRequest Event { get; init; }
        public required string PublicId { get; init; }
    }

    public sealed class
        Handler : IRequestHandler<Command, OneOf<UpdateEventResponse, ValidationResult, NotFound>>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly UpdateEventRequestValidator _validator;

        public Handler(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
            UpdateEventRequestValidator validator)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<OneOf<UpdateEventResponse, ValidationResult, NotFound>> Handle(Command request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Event, cancellationToken);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var user = await _userManager.FindByIdAsync(
                _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var eventEntity = await _context.Events
                .Where(ev => ev.Participants.Any(p => p.AppUserId == user!.Id && p.IsHost))
                .Include(ev => ev.Participants)
                .FirstOrDefaultAsync(ev => ev.PublicId == request.PublicId, cancellationToken);

            if (eventEntity is null)
            {
                return new NotFound();
            }

            eventEntity.UpdateWith(request.Event);
            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateEventResponse(eventEntity.MapToUpdateEventDto());
        }
    }
}