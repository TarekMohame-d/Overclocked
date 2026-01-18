using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.AuthenticationUseCases.Login;

public record LoginRequest : IRequest<AuthResponse>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required Guid DeviceId { get; init; }
}
