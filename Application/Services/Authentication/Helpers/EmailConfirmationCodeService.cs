using System.Security.Cryptography;
using System.Text;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Authentication.Helpers.Interfaces;
using Domain.Entities;

namespace Application.Services.Authentication.Helpers;

public class EmailConfirmationCodeService(
    IEmailConfirmationCodeHasher emailConfirmationCodeHasher,
    IEmailConfirmationCodeRepository emailConfirmationCodeRepository)
    : IEmailConfirmationCodeService
{
    public bool VerifyEmailConfirmationCode(string code, string codeHash) =>
        emailConfirmationCodeHasher.Verify(code, codeHash);
    public async Task<EmailConfirmationCode?> GetEmailConfirmationCodeAsync(
        Guid userId,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        return await emailConfirmationCodeRepository
            .SingleOrDefaultAsync(x => x.UserId == userId, asNoTracking: asNoTracking, cancellationToken);
    }

    public async Task<string> CreateEmailConfirmationCodeAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var plainCode = GenerateVerificationCode();
        var codeHash = emailConfirmationCodeHasher.Hash(plainCode);

        var confirmationCode = new EmailConfirmationCode
        {
            CodeHash = codeHash,
            UserId = userId,
            ExpiredAt = DateTime.UtcNow.AddMinutes(10)
        };

        await emailConfirmationCodeRepository.AddAsync(confirmationCode, cancellationToken);

        return plainCode;
    }

    public void InvalidateEmailConfirmationCode(EmailConfirmationCode emailConfirmationCode)
    {
        emailConfirmationCode.IsUsed = true;
        emailConfirmationCode.UpdatedAt = DateTime.UtcNow;
    }

    public string UpdateEmailConfirmationCode(EmailConfirmationCode emailConfirmationCode)
    {
        var plainCode = GenerateVerificationCode();
        var codeHash = emailConfirmationCodeHasher.Hash(plainCode);

        emailConfirmationCode.IsUsed = false;
        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddMinutes(10);
        emailConfirmationCode.CodeHash = codeHash;
        emailConfirmationCode.UpdatedAt = DateTime.UtcNow;

        return plainCode;
    }

    private static string GenerateVerificationCode(int length = 6)
    {
        const string ValidChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        var builder = new StringBuilder(length);

        for(var i = 0; i < length; i++)
        {
            // Get a cryptographically secure random index
            var index = RandomNumberGenerator.GetInt32(0, ValidChars.Length);
            builder.Append(ValidChars[index]);
        }

        return builder.ToString();
    }
}
