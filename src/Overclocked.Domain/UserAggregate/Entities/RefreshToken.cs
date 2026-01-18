using System.ComponentModel.DataAnnotations.Schema;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate.Entities;

public sealed class RefreshToken : Entity<RefreshTokenId>
{
    private RefreshToken() { }

    private RefreshToken(RefreshTokenId id, Guid deviceId, string tokenHash)
        : base(id)
    {
        DeviceId = deviceId;
        TokenHash = tokenHash;

        ExpiredAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenBaseDays);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public UserId UserId { get; private set; } = null!;

    [NotMapped]
    public const int RefreshTokenBaseDays = 21;
    public Guid DeviceId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiredAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<RefreshToken> Create(Guid deviceId, string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result.Failure<RefreshToken>(AuthenticationErrors.RefreshTokenIsRequired);

        if (deviceId == Guid.Empty)
            return Result.Failure<RefreshToken>(AuthenticationErrors.InvalidDeviceId);

        var refreshToken = new RefreshToken(RefreshTokenId.Create(), deviceId, tokenHash);

        return Result.Success(refreshToken);
    }

    public Result Update(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result.Failure(AuthenticationErrors.RefreshTokenIsRequired);

        TokenHash = tokenHash;
        ExpiredAt = DateTimeOffset.UtcNow.AddDays(RefreshTokenBaseDays);
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
