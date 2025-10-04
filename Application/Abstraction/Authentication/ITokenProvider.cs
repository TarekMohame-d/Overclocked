namespace Application.Abstraction.Authentication;

public interface ITokenProvider
{
    // string GenerateToken(TokenClaims tokenClaims);
    string GenerateRefreshToken();
}
