using Overclocked.Application.Authentication.Commands.Common;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands
{
    public async Task<Result<AuthResponse>> RefreshTokenCommandHandler(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        (Guid userId, string deviceId)? claims = tokenReaderService
            .GetUserIdAndDeviceIdFromToken(command.AccessToken);

        if(claims is null)
        {
            return Result<AuthResponse>.Failure(AuthenticationErrors.InvalidAccessToken);
        }

        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Id == UserId.Create(claims.Value.userId),
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if(user is null)
        {
            return Result<AuthResponse>.Failure(AuthenticationErrors.InvalidAccessToken);
        }

        var oldRefreshTokenHash = user.RefreshTokens
            .First(x => x.DeviceId == claims.Value.deviceId).TokenHash;

        var isValid = oldRefreshTokenHash is not null && refreshTokenHasher
            .Verify(command.RefreshToken, oldRefreshTokenHash);

        if(!isValid)
        {
            return Result<AuthResponse>.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        List<string> permissions = await permissionRepository
            .GetPermissionsByRoleIdAsync(user.RoleId, cancellationToken);

        var tokenClaims = new TokenClaims(
            user.Id.Value.ToString(),
            user.Email,
            claims.Value.deviceId,
            ((RoleType)user.RoleId.Value).ToString(),
            permissions);

        var accessToken = tokenProvider.GenerateAccessToken(tokenClaims);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var refreshTokenHash = refreshTokenHasher.Hash(refreshToken);

        DateTime expiredAt = user.CreateRefreshToken(claims.Value.deviceId, refreshTokenHash);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiredAt = expiredAt
        };

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(authResponse);
    }
}
