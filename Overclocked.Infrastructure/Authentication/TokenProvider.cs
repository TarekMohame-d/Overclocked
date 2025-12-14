using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.Common;
using Overclocked.Application.Common.Constants;
using Overclocked.Infrastructure.Configurations;

namespace Overclocked.Infrastructure.Authentication;

internal sealed class TokenProvider(IOptions<JwtSettings> options) : ITokenProvider
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public string GenerateAccessToken(TokenClaims tokenClaims)
    {
        var claims = new List<Claim>
        {
            new(ClaimsConstants.NameIdentifier, tokenClaims.UserId),
            new(ClaimsConstants.Email, tokenClaims.Email),
            new(ClaimsConstants.DeviceId, tokenClaims.DeviceId),
            new(ClaimsConstants.Role, tokenClaims.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach(var permission in tokenClaims.Permissions)
        {
            // Use a consistent custom claim type for permissions
            claims.Add(new Claim(ClaimsConstants.Permission, permission));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return accessToken;
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32]; // 256 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}
