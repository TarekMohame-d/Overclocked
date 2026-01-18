namespace Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;

public record AuthResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset ExpiredAt { get; init; }
}
