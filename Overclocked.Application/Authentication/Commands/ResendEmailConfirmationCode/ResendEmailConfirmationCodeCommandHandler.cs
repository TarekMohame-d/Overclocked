using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;

public class ResendEmailConfirmationCodeCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService) : ICommandHandler<ResendEmailConfirmationCodeCommand>
{
    public async Task<Result> Handle(ResendEmailConfirmationCodeCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if(user is null)
        {
            return Result.Success(); // to not expose user existence
        }

        if(user.EmailConfirmed)
        {
            return Result.Failure(AuthenticationErrors.EmailAlreadyConfirmed);
        }

        var code = emailConfirmationCodeService.GenerateVerificationCode();
        var codeHash = emailConfirmationCodeService.Hash(code);
        user.ResendEmailConfirmationCode(code, codeHash);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
