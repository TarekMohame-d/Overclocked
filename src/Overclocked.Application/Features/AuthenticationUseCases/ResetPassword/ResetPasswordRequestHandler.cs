using System.Text.RegularExpressions;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.AuthenticationUseCases.ResetPassword;

public class ResetPasswordRequestHandler(
    IAuthenticationRepository authenticationRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailConfirmationCodeService emailConfirmationCodeService
) : IRequestHandler<ResetPasswordRequest>
{
    public async Task<Result> Handle(ResetPasswordRequest request, CancellationToken ct)
    {
        User? user = await authenticationRepository.GetByEmailAsync(request.Email, ct);

        if (user is null)
            return Result.Failure(AuthenticationErrors.InvalidResetPasswordCredentials);

        if (!user.IsActive)
            return Result.Failure(AuthenticationErrors.UserIsInactive);

        if (user.EmailConfirmationCode.ExpiredAt < DateTimeOffset.UtcNow || user.EmailConfirmationCode.IsUsed)
            return Result.Failure(AuthenticationErrors.EmailConfirmationCodeExpired);

        var isValid = emailConfirmationCodeService.Verify(request.Code, user.EmailConfirmationCode.CodeHash);

        if (!isValid)
            return Result.Failure(AuthenticationErrors.InvalidResetPasswordCredentials);

        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        if (string.IsNullOrWhiteSpace(request.Password) || !(regex?.IsMatch(request.Password) ?? false))
            return Result.Failure(AuthenticationErrors.InvalidPassword);

        var passwordHash = passwordHasher.Hash(request.Password);
        user.UpdatePassword(passwordHash);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
