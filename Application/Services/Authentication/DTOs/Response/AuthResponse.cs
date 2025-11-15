namespace Application.Services.Authentication.DTOs.Response;

public record AuthResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime RefreshTokenExpiration { get; init; }
}
