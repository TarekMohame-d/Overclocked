using Application.Services.Authentication;

namespace Application.Abstraction.Services;

public interface ITokenProvider
{
    string GenerateToken(TokenClaims tokenClaims);
    string GenerateRefreshToken();
}
