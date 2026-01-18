using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.Common;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.AuthenticationUseCases.RefreshToken;

public class RefreshTokenRequestHandler(
    IAuthenticationRepository authenticationRepository,
    IUnitOfWork unitOfWork,
    IRefreshTokenHasher refreshTokenHasher,
    ITokenReaderService tokenReaderService,
    ITokenProvider tokenProvider
) : IRequestHandler<RefreshTokenRequest, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenRequest request, CancellationToken ct)
    {
        (Guid userId, Guid deviceId)? claims = tokenReaderService.GetUserIdAndDeviceIdFromToken(request.AccessToken);

        if (claims is null)
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidAccessToken);

        User? user = await authenticationRepository.GetWithRefreshTokensAsync(UserId.Create(claims.Value.userId), ct);

        if (user is null)
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidAccessToken);

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(AuthenticationErrors.UserIsInactive);

        var oldRefreshTokenHash = user.RefreshTokens.First(x => x.DeviceId == claims.Value.deviceId).TokenHash;

        var isValid = oldRefreshTokenHash is not null && refreshTokenHasher.Verify(request.RefreshToken, oldRefreshTokenHash);

        if (!isValid)
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidRefreshToken);

        List<string> permissions = await authenticationRepository.GetPermissionsAsync(user.Role, ct);

        var tokenClaims = new TokenClaims(
            user.Id.Value.ToString(),
            user.Email,
            claims.Value.deviceId.ToString(),
            user.Role.ToString(),
            permissions
        );

        var accessToken = tokenProvider.GenerateAccessToken(tokenClaims);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var refreshTokenHash = refreshTokenHasher.Hash(refreshToken);

        Result<DateTimeOffset> result = user.CreateRefreshToken(claims.Value.deviceId, refreshTokenHash);

        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiredAt = result.Value,
        };

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(authResponse);
    }
}
