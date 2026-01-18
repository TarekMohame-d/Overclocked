using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.AuthenticationUseCases.ConfirmEmail;

public class ConfirmEmailRequestHandler(
    IAuthenticationRepository authenticationRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService
) : IRequestHandler<ConfirmEmailRequest>
{
    public async Task<Result> Handle(ConfirmEmailRequest request, CancellationToken ct)
    {
        User? user = await authenticationRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
            return Result.Failure(AuthenticationErrors.InvalidConfirmationCodeCredentials);

        if (user.EmailConfirmed || user.EmailConfirmationCode.IsUsed)
            return Result.Failure(AuthenticationErrors.EmailAlreadyConfirmed);

        if (user.EmailConfirmationCode.ExpiredAt < DateTimeOffset.UtcNow)
            return Result.Failure(AuthenticationErrors.EmailConfirmationCodeExpired);

        var isValid = emailConfirmationCodeService.Verify(request.Code, user.EmailConfirmationCode.CodeHash);

        if (!isValid)
            return Result.Failure(AuthenticationErrors.InvalidConfirmationCodeCredentials);

        user.ConfirmEmail();

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
