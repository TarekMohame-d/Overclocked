namespace Overclocked.Application.Features.AuthenticationUseCases.DTOs.Requests;

public record RefreshTokenRequestDto(string AccessToken, string RefreshToken);
