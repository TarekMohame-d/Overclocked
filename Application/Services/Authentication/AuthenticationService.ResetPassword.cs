using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Domain.Entities;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        // TODO: refactor to use user service instead of directly using user repository
        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (user is null)
            return Result.Failure(Errors.InvalidResetPasswordCredentials);

        EmailConfirmationCode emailConfirmationCode =
            await emailConfirmationCodeService.GetEmailConfirmationCodeAsync(user.Id, cancellationToken)
            ?? throw new Exception("Confirmation code not found.");

        if (emailConfirmationCode.ExpiredAt < DateTime.UtcNow || emailConfirmationCode.IsUsed)
            return Result.Failure(Errors.EmailConfirmationCodeExpired);

        var isValid = emailConfirmationCodeHasher.Verify(request.Code, emailConfirmationCode.CodeHash);

        if (!isValid)
            return Result.Failure(Errors.InvalidResetPasswordCredentials);

        user.PasswordHash = passwordHasher.Hash(request.Password);
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;
        userRepository.Update(user);

        emailConfirmationCode.IsUsed = true;
        emailConfirmationCode.UpdatedAt = DateTime.UtcNow;
        emailConfirmationCodeService.InvalidateEmailConfirmationCode(emailConfirmationCode);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
