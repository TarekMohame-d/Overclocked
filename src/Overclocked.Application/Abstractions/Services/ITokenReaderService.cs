namespace Overclocked.Application.Abstractions.Services;

public interface ITokenReaderService
{
    (Guid userId, Guid deviceId)? GetUserIdAndDeviceIdFromToken(string accessToken);
    Dictionary<string, string>? ExtractClaimsFromToken(string accessToken);
}
