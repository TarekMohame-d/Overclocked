using Overclocked.Application.Authentication.Commands.Common;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands
{
    public async Task<Result<AuthResponse>> LoginCommandHandler(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        var passwordHash =
            user?.PasswordHash
            ?? "1616DCA463F65B974204B57CBF28BD29F1FD75F8ECAB30836B003DD2D88820E4-AB3D8D1DAE09FF974C46FDF6BBD31A36";

        var isValid = passwordHasher.Verify(command.Password, passwordHash);

        if(user is null || !isValid)
        {
            return Result<AuthResponse>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        if(!user.EmailConfirmed)
        {
            var code = emailConfirmationCodeService.GenerateVerificationCode();
            var codeHash = emailConfirmationCodeService.Hash(code);
            user.ResendEmailConfirmationCode(code, codeHash);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Failure(AuthenticationErrors.EmailNotConfirmed);
        }

        List<string> permissions = await permissionRepository
            .GetPermissionsByRoleIdAsync(user.RoleId, cancellationToken);

        var tokenClaims = new TokenClaims(
            user.Id.Value.ToString(),
            user.Email,
            command.DeviceId,
            ((RoleType)user.RoleId.Value).ToString(),
            permissions);

        var accessToken = tokenProvider.GenerateAccessToken(tokenClaims);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var refreshTokenHash = refreshTokenHasher.Hash(refreshToken);

        DateTime expiredAt = user.CreateRefreshToken(command.DeviceId, refreshTokenHash);

        var authResponse = new AuthResponse(accessToken, refreshToken, expiredAt);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(authResponse);
    }
}
