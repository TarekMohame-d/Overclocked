using System.Net;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands
{
    public async Task<Result> ConfirmEmailCommandHandler(
        ConfirmEmailCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if(user is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidConfirmationCodeCredentials);
        }

        if(user.EmailConfirmed || user.EmailConfirmationCode.IsUsed)
        {
            return Result.Failure(AuthenticationErrors.EmailAlreadyConfirmed, HttpStatusCode.Conflict);
        }

        if(user.EmailConfirmationCode.ExpiredAt < DateTime.UtcNow)
        {
            return Result.Failure(AuthenticationErrors.EmailConfirmationCodeExpired);
        }

        var isValid = emailConfirmationCodeService.Verify(command.Code, user.EmailConfirmationCode.CodeHash);

        if(!isValid)
        {
            return Result.Failure(AuthenticationErrors.InvalidConfirmationCodeCredentials);
        }

        user.ConfirmEmail();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
