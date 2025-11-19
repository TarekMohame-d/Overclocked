using Application.Abstraction.Messaging;

namespace Application.Services.Authentication.Events;

public record UserRegisteredEvent(string Email, string Code) : IDomainEvent;
