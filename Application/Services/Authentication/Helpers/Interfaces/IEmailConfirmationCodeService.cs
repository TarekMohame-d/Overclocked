using Domain.Entities;

namespace Application.Services.Authentication.Helpers.Interfaces;

public interface IEmailConfirmationCodeService
{
    Task<EmailConfirmationCode?> GetEmailConfirmationCodeAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task<string> CreateEmailConfirmationCodeAsync(Guid userId, CancellationToken cancellationToken = default);

    void InvalidateEmailConfirmationCode(EmailConfirmationCode emailConfirmationCode);
    string UpdateEmailConfirmationCode(EmailConfirmationCode emailConfirmationCode);

    Task DeleteEmailConfirmationCodeAsync(Guid userId, CancellationToken cancellationToken = default);
}
