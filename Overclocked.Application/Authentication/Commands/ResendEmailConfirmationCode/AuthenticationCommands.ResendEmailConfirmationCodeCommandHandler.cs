using System.Net;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands
{
    public async Task<Result> ResendConfirmationCodeCommandHandler(
        ResendEmailConfirmationCodeCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if(user is null)
        {
            return Result.Success(); // to not expose user existence
        }

        if(user.EmailConfirmed)
        {
            return Result.Failure(AuthenticationErrors.EmailAlreadyConfirmed, HttpStatusCode.Conflict);
        }

        var code = emailConfirmationCodeService.GenerateVerificationCode();
        var codeHash = emailConfirmationCodeService.Hash(code);
        user.ResendEmailConfirmationCode(code, codeHash);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
