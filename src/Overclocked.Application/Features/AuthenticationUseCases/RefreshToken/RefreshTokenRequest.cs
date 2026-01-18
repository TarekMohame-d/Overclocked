using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.AuthenticationUseCases.RefreshToken;

public record RefreshTokenRequest : IRequest<AuthResponse>
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
