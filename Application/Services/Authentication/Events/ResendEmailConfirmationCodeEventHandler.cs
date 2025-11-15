using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Authentication.Events;

public class ResendEmailConfirmationCodeEventHandler(
    IBackgroundJobClient jobClient,
    IEmailService emailService)
    : IEventHandler<ResendEmailConfirmationCodeEvent>
{
    public Task HandleAsync(ResendEmailConfirmationCodeEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((ResendEmailConfirmationCodeEvent)domainEvent, cancellationToken);
}
