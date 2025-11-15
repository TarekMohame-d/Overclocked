using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Authentication.Helpers.Interfaces;
using Domain.Entities;

namespace Application.Services.Authentication.Helpers;

public class RefreshTokenService(
    ITokenProvider tokenProvider,
    IRefreshTokenHasher refreshTokenHasher,
    IRefreshTokenRepository refreshTokenRepository)
    : IRefreshTokenService
{
    private const int RefreshTokenBaseDays = 14;

    public async Task<(string refreshToken, DateTime expiredAt)> CreateRefreshTokenAsync(
        Guid userId,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        await refreshTokenRepository.DeleteWhereAsync(
            x => x.UserId == userId && x.DeviceId == deviceId,
            cancellationToken: cancellationToken);

        var token = tokenProvider.GenerateRefreshToken();
        var tokenHash = refreshTokenHasher.Hash(token);
        DateTime expiredAt = DateTime.UtcNow.AddDays(RefreshTokenBaseDays);

        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            UserId = userId,
            DeviceId = deviceId,
            ExpiredAt = expiredAt
        };
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return (token, expiredAt);
    }

    public async Task<(string refreshToken, DateTime expiredAt)> UpdateRefreshTokenAsync(
        Guid userId,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? existingToken = await refreshTokenRepository.SingleOrDefaultAsync(
            x => x.UserId == userId && x.DeviceId == deviceId,
            cancellationToken: cancellationToken) ?? throw new InvalidOperationException(
                "Expected refresh token not found during update. Race condition or unexpected deletion.");

        var token = tokenProvider.GenerateRefreshToken();
        var tokenHash = refreshTokenHasher.Hash(token);
        DateTime expiredAt = DateTime.UtcNow.AddDays(RefreshTokenBaseDays);

        existingToken.TokenHash = tokenHash;
        existingToken.ExpiredAt = expiredAt;
        existingToken.UpdatedAt = DateTime.UtcNow;

        refreshTokenRepository.Update(existingToken);

        return (token, expiredAt);
    }

    public async Task<bool> VerifyRefreshTokenAsync(Guid userId, string deviceId, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? existingToken = await refreshTokenRepository.SingleOrDefaultAsync(x => x.UserId == userId
        && x.DeviceId == deviceId, cancellationToken: cancellationToken);

        return existingToken is not null && existingToken.ExpiredAt > DateTime.UtcNow
            && refreshTokenHasher.Verify(refreshToken, existingToken.TokenHash);
    }
}
