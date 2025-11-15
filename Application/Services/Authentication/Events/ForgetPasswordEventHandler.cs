using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Authentication.Events;

public class ForgetPasswordEventHandler(
    IBackgroundJobClient jobClient,
    IEmailService emailService)
    : IEventHandler<ForgetPasswordEvent>
{
    public Task HandleAsync(ForgetPasswordEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => emailService.SendConfirmationCode(domainEvent.Email, domainEvent.Code));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((ForgetPasswordEvent)domainEvent, cancellationToken);
}
