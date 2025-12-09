using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands
{
    public async Task<Result> ResetPasswordCommandHandler(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if(user is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidResetPasswordCredentials);
        }

        if(user.EmailConfirmationCode.ExpiredAt < DateTime.UtcNow || user.EmailConfirmationCode.IsUsed)
        {
            return Result.Failure(AuthenticationErrors.EmailConfirmationCodeExpired);
        }

        var isValid = emailConfirmationCodeService.Verify(command.Code, user.EmailConfirmationCode.CodeHash);

        if(!isValid)
        {
            return Result.Failure(AuthenticationErrors.InvalidResetPasswordCredentials);
        }

        var passwordHash = passwordHasher.Hash(command.Password);
        user.UpdatePassword(passwordHash);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
