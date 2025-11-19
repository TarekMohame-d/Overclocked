using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Authentication.Events;

public class EnqueueConfirmationEmailHandler(IBackgroundJobClient jobClient, IEmailService emailService)
    : IEventHandler<UserRegisteredEvent>
{
    public Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((UserRegisteredEvent)domainEvent, cancellationToken);
}
