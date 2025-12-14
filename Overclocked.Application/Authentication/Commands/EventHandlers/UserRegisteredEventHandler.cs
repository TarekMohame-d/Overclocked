using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.UserAggregate.Events;

namespace Overclocked.Application.Authentication.Commands.EventHandlers;

public class UserRegisteredEventHandler(IEmailService emailService) : IDomainEventHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code);
    }
}
