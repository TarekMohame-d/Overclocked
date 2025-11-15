using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Events;
using Domain.Entities;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result> ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
    {
        // TODO: refactor to use user service instead of directly using user repository
        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Email == request.Email, cancellationToken: cancellationToken);

        if (user is null)
            return Result.Success(); // to not expose user existence

        EmailConfirmationCode? emailConfirmationCode =
            await emailConfirmationCodeService.GetEmailConfirmationCodeAsync(user.Id, cancellationToken);

        if (emailConfirmationCode is null)
            return Result.Success(); // to not expose user existence

        var code = emailConfirmationCodeService.UpdateEmailConfirmationCode(emailConfirmationCode);

        await unitOfWork.CompleteAsync(cancellationToken);

        ForgetPasswordEvent forgetPasswordEvent = new(user.Email, code);
        await eventDispatcher.DispatchAsync(forgetPasswordEvent, cancellationToken);


        return Result.Success();
    }
}
