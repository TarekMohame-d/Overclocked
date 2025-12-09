using Overclocked.Application.Abstraction.Services;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.UserAggregate.Events;

namespace Overclocked.Application.Authentication.Commands.EventHandlers;

public class UserEmailConfirmationCodeResendEventHandler(IEmailService emailService)
    : IDomainEventHandler<UserEmailConfirmationCodeResendEvent>
{
    public async Task Handle(
        UserEmailConfirmationCodeResendEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code);
    }
}
