using System.IdentityModel.Tokens.Jwt;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Infrastructure.Services;

internal sealed class TokenReaderService : ITokenReaderService
{
    public IDictionary<string, string>? GetClaimsFromToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();

        if(!handler.CanReadToken(accessToken))
            return null;

        JwtSecurityToken token = handler.ReadJwtToken(accessToken);

        return token
            .Claims.Where(c => c.Type is ClaimsConstants.NameIdentifier or ClaimsConstants.DeviceId)
            .ToDictionary(c => c.Type, c => c.Value);
    }
}
