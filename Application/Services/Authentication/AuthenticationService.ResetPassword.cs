using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Authentication.DTOs.Request;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        // TODO: refactor to use user service instead of directly using user repository
        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Email == request.Email,
            asNoTracking: false,
            cancellationToken);

        if(user is null)
            return Result.Failure(Errors.InvalidResetPasswordCredentials);

        EmailConfirmationCode emailConfirmationCode =
            await emailConfirmationCodeService.GetEmailConfirmationCodeAsync(user.Id, false, cancellationToken)
            ?? throw new EmailConfirmationCodeNotExistException(user.Id);

        if(emailConfirmationCode.ExpiredAt < DateTime.UtcNow || emailConfirmationCode.IsUsed)
            return Result.Failure(Errors.EmailConfirmationCodeExpired);

        var isValid = emailConfirmationCodeService
            .VerifyEmailConfirmationCode(request.Code, emailConfirmationCode.CodeHash);

        if(!isValid)
            return Result.Failure(Errors.InvalidResetPasswordCredentials);

        user.PasswordHash = passwordHasher.Hash(request.Password);
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;

        emailConfirmationCodeService.InvalidateEmailConfirmationCode(emailConfirmationCode);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
