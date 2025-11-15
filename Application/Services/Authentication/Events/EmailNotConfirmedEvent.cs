using Application.Abstraction.Messaging;

namespace Application.Services.Authentication.Events;

public record EmailNotConfirmedEvent(string Email, Guid UserId) : IDomainEvent;
