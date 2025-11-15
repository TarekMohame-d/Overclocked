using Application.Abstraction.Messaging;

namespace Application.Services.Authentication.Events;

public record ForgetPasswordEvent(string Email, string Code) : IDomainEvent;
