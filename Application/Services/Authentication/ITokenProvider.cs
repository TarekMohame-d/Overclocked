namespace Application.Services.Authentication;

public interface ITokenProvider
{
    // string GenerateToken(TokenClaims tokenClaims);
    string GenerateRefreshToken();
}
