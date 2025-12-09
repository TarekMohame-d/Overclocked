namespace Overclocked.Application.Abstraction.Services;

public interface ITokenReaderService
{
    (Guid userId, string deviceId)? GetUserIdAndDeviceIdFromToken(string accessToken);
    Dictionary<string, string>? ExtractClaimsFromToken(string accessToken);
}
