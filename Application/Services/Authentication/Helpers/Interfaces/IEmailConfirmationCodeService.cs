using Domain.Entities;

namespace Application.Services.Authentication.Helpers.Interfaces;

public interface IEmailConfirmationCodeService
{
    bool VerifyEmailConfirmationCode(string code, string codeHash);
    Task<EmailConfirmationCode?> GetEmailConfirmationCodeAsync(
        Guid userId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<string> CreateEmailConfirmationCodeAsync(Guid userId, CancellationToken cancellationToken = default);
    void InvalidateEmailConfirmationCode(EmailConfirmationCode emailConfirmationCode);
    string UpdateEmailConfirmationCode(EmailConfirmationCode emailConfirmationCode);
}
