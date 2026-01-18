using Overclocked.Application.Features.AuthenticationUseCases.Common;

namespace Overclocked.Application.Abstractions.Services;

public interface ITokenProvider
{
    string GenerateAccessToken(TokenClaims tokenClaims);
    string GenerateRefreshToken();
}
