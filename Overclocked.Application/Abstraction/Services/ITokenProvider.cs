using Overclocked.Application.Authentication.Commands.Common;

namespace Overclocked.Application.Abstraction.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(TokenClaims tokenClaims);
    string GenerateRefreshToken();
}
