using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailConfirmationCodeService emailConfirmationCodeService) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
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
