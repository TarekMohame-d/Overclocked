namespace Overclocked.Application.Features.AuthenticationUseCases.DTOs.Requests;

public record LoginRequestDto(string Email, string Password, Guid DeviceId);
