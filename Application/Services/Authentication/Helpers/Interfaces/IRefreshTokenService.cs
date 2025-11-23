namespace Application.Services.Authentication.Helpers.Interfaces;

public interface IRefreshTokenService
{
    Task<(string refreshToken, DateTime expiredAt)> CreateRefreshTokenAsync(
        Guid userId,
        string deviceId,
        CancellationToken cancellationToken = default);
    Task<(string refreshToken, DateTime expiredAt)> UpdateRefreshTokenAsync(
        Guid userId,
        string deviceId,
        CancellationToken cancellationToken = default);
    Task<bool> VerifyRefreshTokenAsync(
        Guid userId,
        string deviceId,
        string refreshToken,
        CancellationToken cancellationToken = default);
}
