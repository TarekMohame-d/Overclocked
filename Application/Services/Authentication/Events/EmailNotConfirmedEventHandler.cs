using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Authentication.Helpers.Interfaces;
using Domain.Entities;
using Hangfire;

namespace Application.Services.Authentication.Events;

public class EmailNotConfirmedEventHandler(
    IEmailConfirmationCodeService emailConfirmationCodeService,
    IUnitOfWork unitOfWork,
    IBackgroundJobClient jobClient,
    IEmailService emailService)
    : IEventHandler<EmailNotConfirmedEvent>
{
    public async Task HandleAsync(EmailNotConfirmedEvent domainEvent, CancellationToken cancellationToken)
    {
        EmailConfirmationCode emailConfirmationCode =
            await emailConfirmationCodeService.GetEmailConfirmationCodeAsync(domainEvent.UserId, cancellationToken)
            ?? throw new Exception("Confirmation code not found");

        // Check if the code is expired and generate a new one
        if (emailConfirmationCode.ExpiredAt <= DateTime.UtcNow.AddMinutes(5))
        {
            var code = emailConfirmationCodeService.UpdateEmailConfirmationCode(emailConfirmationCode);

            await unitOfWork.CompleteAsync(cancellationToken);

            jobClient.Enqueue(() => emailService.SendConfirmationCode(domainEvent.Email, code));
        }
    }

    public async Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        await HandleAsync((EmailNotConfirmedEvent)domainEvent, cancellationToken);
}
