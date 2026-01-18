using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.AuthenticationUseCases.EventHandlers;

public class UserEmailConfirmationCodeResendEventHandler(IEmailService emailService)
    : IDomainEventHandler<UserEmailConfirmationCodeResendEvent>
{
    public async Task Handle(UserEmailConfirmationCodeResendEvent domainEvent, CancellationToken ct = default) =>
        await emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code);
}
