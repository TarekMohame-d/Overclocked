using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.Common;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Application.Authentication.Commands.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService,
    IPasswordHasher passwordHasher,
    IRefreshTokenHasher refreshTokenHasher,
    ITokenProvider tokenProvider) : ICommandHandler<LoginCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        var passwordHash =
            user?.PasswordHash
            ?? "1616DCA463F65B974204B57CBF28BD29F1FD75F8ECAB30836B003DD2D88820E4-AB3D8D1DAE09FF974C46FDF6BBD31A36";

        var isValid = passwordHasher.Verify(command.Password, passwordHash);

        if(user is null || !isValid)
        {
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidCredentials);
        }

        if(!user.EmailConfirmed)
        {
            var code = emailConfirmationCodeService.GenerateVerificationCode();
            var codeHash = emailConfirmationCodeService.Hash(code);
            user.ResendEmailConfirmationCode(code, codeHash);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure<AuthResponse>(AuthenticationErrors.EmailNotConfirmed);
        }

        List<string> permissions = await userRepository
            .GetPermissionsByRoleAsync(user.Role, cancellationToken);

        var tokenClaims = new TokenClaims(
            user.Id.Value.ToString(),
            user.Email,
            command.DeviceId,
            user.Role.ToString(),
            permissions);

        var accessToken = tokenProvider.GenerateAccessToken(tokenClaims);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var refreshTokenHash = refreshTokenHasher.Hash(refreshToken);

        DateTime expiredAt = user.CreateRefreshToken(command.DeviceId, refreshTokenHash);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiredAt = expiredAt
        };

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(authResponse);
    }
}
