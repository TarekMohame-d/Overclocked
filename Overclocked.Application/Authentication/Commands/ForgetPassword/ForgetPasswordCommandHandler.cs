using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands.ForgetPassword;

public class ForgetPasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService) : ICommandHandler<ForgetPasswordCommand>
{
    public async Task<Result> Handle(ForgetPasswordCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if(user is null)
        {
            return Result.Success(); // to not expose user existence
        }

        var code = emailConfirmationCodeService.GenerateVerificationCode();
        var codeHash = emailConfirmationCodeService.Hash(code);
        user.ResendEmailConfirmationCode(code, codeHash);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
