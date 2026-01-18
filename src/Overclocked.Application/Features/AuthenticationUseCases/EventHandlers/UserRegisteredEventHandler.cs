using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.AuthenticationUseCases.EventHandlers;

public class UserRegisteredEventHandler(IEmailService emailService) : IDomainEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent domainEvent, CancellationToken ct = default) =>
        await emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code);
}
