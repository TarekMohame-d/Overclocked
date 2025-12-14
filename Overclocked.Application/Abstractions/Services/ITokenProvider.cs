using Overclocked.Application.Authentication.Commands.Common;

namespace Overclocked.Application.Abstractions.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(TokenClaims tokenClaims);
    string GenerateRefreshToken();
}
