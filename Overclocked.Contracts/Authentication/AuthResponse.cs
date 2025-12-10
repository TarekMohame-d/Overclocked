namespace Overclocked.Contracts.Authentication;

public record AuthResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime ExpiredAt { get; init; }
}
