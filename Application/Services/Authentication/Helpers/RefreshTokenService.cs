using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Authentication.Helpers.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.Authentication.Helpers;

public class RefreshTokenService(
    ITokenProvider tokenProvider,
    IRefreshTokenHasher refreshTokenHasher,
    IRefreshTokenRepository refreshTokenRepository
) : IRefreshTokenService
{
    private const int RefreshTokenBaseDays = 14;

    public async Task<(string refreshToken, DateTime expiredAt)> CreateRefreshTokenAsync(
        Guid userId,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? refreshToken = await refreshTokenRepository.SingleOrDefaultAsync(
            x => x.UserId == userId && x.DeviceId == deviceId,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        var token = tokenProvider.GenerateRefreshToken();
        var tokenHash = refreshTokenHasher.Hash(token);
        DateTime expiredAt = DateTime.UtcNow.AddDays(RefreshTokenBaseDays);

        if(refreshToken is not null)
        {
            refreshToken.TokenHash = tokenHash;
            refreshToken.ExpiredAt = expiredAt;
            refreshToken.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            refreshToken = new()
            {
                TokenHash = tokenHash,
                UserId = userId,
                DeviceId = deviceId,
                ExpiredAt = expiredAt
            };

            await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        }

        return (token, expiredAt);
    }

    public async Task<(string refreshToken, DateTime expiredAt)> UpdateRefreshTokenAsync(
        Guid userId,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? refreshToken = await refreshTokenRepository.SingleOrDefaultAsync(
            x => x.UserId == userId && x.DeviceId == deviceId,
            asNoTracking: false,
            cancellationToken: cancellationToken)
            ?? throw new RefreshTokenNotExistException(userId, deviceId);

        var token = tokenProvider.GenerateRefreshToken();
        var tokenHash = refreshTokenHasher.Hash(token);
        DateTime expiredAt = DateTime.UtcNow.AddDays(RefreshTokenBaseDays);

        refreshToken.TokenHash = tokenHash;
        refreshToken.ExpiredAt = expiredAt;
        refreshToken.UpdatedAt = DateTime.UtcNow;

        return (token, expiredAt);
    }

    public async Task<bool> VerifyRefreshTokenAsync(
        Guid userId,
        string deviceId,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? existingToken = await refreshTokenRepository.SingleOrDefaultAsync(
            x => x.UserId == userId && x.DeviceId == deviceId,
            cancellationToken: cancellationToken);

        return existingToken is not null
            && existingToken.ExpiredAt > DateTime.UtcNow
            && refreshTokenHasher.Verify(refreshToken, existingToken.TokenHash);
    }
}
