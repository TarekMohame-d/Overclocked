using Application.Abstraction.Messaging;

namespace Application.Services.Authentication.Events;

public record ResendEmailConfirmationCodeEvent(string Email, string Code) : IDomainEvent;
