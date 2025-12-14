using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task<Result> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if(user is null)
        {
            return Result.Failure(AuthenticationErrors.InvalidConfirmationCodeCredentials);
        }

        if(user.EmailConfirmed || user.EmailConfirmationCode.IsUsed)
        {
            return Result.Failure(AuthenticationErrors.EmailAlreadyConfirmed);
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
