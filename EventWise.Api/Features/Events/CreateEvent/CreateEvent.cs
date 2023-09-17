using System.Security.Claims;
using EventWise.Api.Features.Events.Domain;
using EventWise.Api.Persistence;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OneOf;

namespace EventWise.Api.Features.Events.CreateEvent;

public static class CreateEvent
{
    public sealed class Command : IRequest<OneOf<CreateEventResponse, ValidationResult>>
    {
        public required CreateEventRequest Event { get; init; }
    }

    public sealed class Handler : IRequestHandler<Command, OneOf<CreateEventResponse, ValidationResult>>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IValidator<CreateEventRequest> _validator;

        public Handler(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IValidator<CreateEventRequest> validator)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<OneOf<CreateEventResponse, ValidationResult>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Event, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                return validationResult;
            }
            
            var eventHost = await FindUser();
            var eventEntity = request.Event.MapToEventEntityWith(eventHost);
            
            await InsertToDatabase(eventEntity, cancellationToken);
            
            var createdEventDto = eventEntity.MapToCreatedEventDto();

            return new CreateEventResponse(createdEventDto);
        }

        private async Task InsertToDatabase(EventEntity eventEntity, CancellationToken cancellationToken)
        {
            await _context.Events.AddAsync(eventEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<AppUser> FindUser()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);
            
            return user!;
        }
    }
}