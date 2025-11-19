namespace Application.Services.Authentication.DTOs.Request;

public record RefreshTokenRequest
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
