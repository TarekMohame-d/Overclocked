using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.AuthenticationUseCases.ResendEmailConfirmationCode;

public class ResendEmailConfirmationCodeRequestHandler(
    IAuthenticationRepository authenticationRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService
) : IRequestHandler<ResendEmailConfirmationCodeRequest>
{
    public async Task<Result> Handle(ResendEmailConfirmationCodeRequest request, CancellationToken ct)
    {
        User? user = await authenticationRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
            return Result.Success(); // to not expose user existence

        if (!user.IsActive)
            return Result.Failure(AuthenticationErrors.UserIsInactive);

        if (user.EmailConfirmed)
            return Result.Failure(AuthenticationErrors.EmailAlreadyConfirmed);

        var code = emailConfirmationCodeService.GenerateVerificationCode();
        var codeHash = emailConfirmationCodeService.Hash(code);
        user.ResendEmailConfirmationCode(code, codeHash);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
