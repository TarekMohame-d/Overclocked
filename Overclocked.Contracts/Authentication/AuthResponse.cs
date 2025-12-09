namespace Overclocked.Contracts.Authentication;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime RefreshTokenExpiration);
