namespace Overclocked.Application.Abstractions.Services;

public interface ITokenReaderService
{
    (Guid userId, string deviceId)? GetUserIdAndDeviceIdFromToken(string accessToken);
    Dictionary<string, string>? ExtractClaimsFromToken(string accessToken);
}
