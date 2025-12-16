using System.ComponentModel.DataAnnotations.Schema;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.UserAggregate.Entities;

public class RefreshToken : Entity<RefreshTokenId>
{
    [NotMapped]
    public const int RefreshTokenBaseDays = 30;
    public string DeviceId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiredAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private RefreshToken()
    {
    }
    private RefreshToken(
        RefreshTokenId id,
        string deviceId,
        string tokenHash) : base(id)
    {
        DeviceId = deviceId;
        TokenHash = tokenHash;

        ExpiredAt = DateTime.UtcNow.AddDays(RefreshTokenBaseDays);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static RefreshToken Create(string deviceId, string tokenHash)
    {
        return new RefreshToken(
            id: RefreshTokenId.Create(),
            deviceId: deviceId,
            tokenHash: tokenHash);
    }

    public void Update(string tokenHash)
    {
        TokenHash = tokenHash;
        ExpiredAt = DateTime.UtcNow.AddDays(RefreshTokenBaseDays);
        UpdatedAt = DateTime.UtcNow;
    }
}
