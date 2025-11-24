using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Events;
using Domain.Entities;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result> ResendEmailConfirmationCodeAsync(
        ResendEmailConfirmationCodeRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: refactor to use user service instead of directly using user repository
        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Email == request.Email,
            cancellationToken: cancellationToken);

        if(user is null)
            return Result.Success(); // to not expose user existence

        if(user.EmailConfirmed)
            return Result.Failure(Errors.EmailAlreadyConfirmed, HttpStatusCode.Conflict);

        EmailConfirmationCode? emailConfirmationCode = await emailConfirmationCodeService
            .GetEmailConfirmationCodeAsync(user.Id, false, cancellationToken);

        var code = emailConfirmationCode is null
            ? await emailConfirmationCodeService.CreateEmailConfirmationCodeAsync(user.Id, cancellationToken)
            : emailConfirmationCodeService.UpdateEmailConfirmationCode(emailConfirmationCode);

        await unitOfWork.CompleteAsync(cancellationToken);

        ResendEmailConfirmationCodeEvent emailConfirmationCodeEvent = new(user.Email, code);
        await eventDispatcher.DispatchAsync(emailConfirmationCodeEvent, cancellationToken);

        return Result.Success();
    }
}
