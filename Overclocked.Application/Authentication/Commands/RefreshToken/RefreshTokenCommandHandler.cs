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
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IRefreshTokenHasher refreshTokenHasher,
    ITokenReaderService tokenReaderService,
    ITokenProvider tokenProvider) : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        (Guid userId, string deviceId)? claims = tokenReaderService
            .GetUserIdAndDeviceIdFromToken(command.AccessToken);

        if(claims is null)
        {
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidAccessToken);
        }

        User? user = await userRepository
            .GetWithRefreshTokensAsync(UserId.Create(claims.Value.userId), cancellationToken);

        if(user is null)
        {
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidAccessToken);
        }

        var oldRefreshTokenHash = user.RefreshTokens
            .First(x => x.DeviceId == claims.Value.deviceId).TokenHash;

        var isValid = oldRefreshTokenHash is not null && refreshTokenHasher
            .Verify(command.RefreshToken, oldRefreshTokenHash);

        if(!isValid)
        {
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidRefreshToken);
        }

        List<string> permissions = await userRepository
            .GetPermissionsByRoleAsync(user.Role, cancellationToken);

        var tokenClaims = new TokenClaims(
            user.Id.Value.ToString(),
            user.Email,
            claims.Value.deviceId,
            user.Role.ToString(),
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

        return Result.Success(authResponse);
    }
}
