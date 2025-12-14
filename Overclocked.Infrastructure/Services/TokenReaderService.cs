using System.IdentityModel.Tokens.Jwt;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;

namespace Overclocked.Infrastructure.Services;

internal sealed class TokenReaderService : ITokenReaderService
{
    public (Guid userId, string deviceId)? GetUserIdAndDeviceIdFromToken(string accessToken)
    {
        Dictionary<string, string>? claims = ExtractClaimsFromToken(accessToken);

        if(claims is null)
        {
            return null;
        }

        claims.TryGetValue(ClaimsConstants.NameIdentifier, out var nameIdentifier);
        claims.TryGetValue(ClaimsConstants.DeviceId, out var deviceId);

        if(string.IsNullOrEmpty(nameIdentifier) || string.IsNullOrEmpty(deviceId))
        {
            return null;
        }

        if(!Guid.TryParse(claims[ClaimsConstants.NameIdentifier], out Guid userId))
        {
            return null;
        }

        return (userId, deviceId);
    }
    public Dictionary<string, string>? ExtractClaimsFromToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();

        if(!handler.CanReadToken(accessToken))
        {
            return null;
        }

        JwtSecurityToken token = handler.ReadJwtToken(accessToken);

        return token
            .Claims.Where(c => c.Type is ClaimsConstants.NameIdentifier or ClaimsConstants.DeviceId)
            .ToDictionary(c => c.Type, c => c.Value);
    }
}
