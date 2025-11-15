using System.Net;
using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Domain.Entities;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        // TODO: refactor to use user service instead of directly using user repository
        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Email.ToLower() == request.Email.ToLower(),
            cancellationToken: cancellationToken);

        if (user is null)
            return Result.Failure(Errors.InvalidConfirmationCodeCredentials);

        EmailConfirmationCode emailConfirmationCode =
            await emailConfirmationCodeService.GetEmailConfirmationCodeAsync(user.Id, cancellationToken)
            ?? throw new Exception("Confirmation code not found.");

        if (user.EmailConfirmed || emailConfirmationCode.IsUsed)
            return Result.Failure(Errors.EmailAlreadyConfirmed, HttpStatusCode.Conflict);

        if (emailConfirmationCode.ExpiredAt < DateTime.UtcNow)
            return Result.Failure(Errors.EmailConfirmationCodeExpired);

        var isValid = emailConfirmationCodeHasher.Verify(request.Code, emailConfirmationCode.CodeHash);

        if (!isValid)
            return Result.Failure(Errors.InvalidConfirmationCodeCredentials);

        emailConfirmationCode.IsUsed = true;
        emailConfirmationCode.UpdatedAt = DateTime.UtcNow;
        emailConfirmationCodeService.InvalidateEmailConfirmationCode(emailConfirmationCode);

        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;
        userRepository.Update(user);

        // TODO: Implement Create user cart and wishlist

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
