using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.AuthenticationUseCases.ResetPassword;

public record ResetPasswordRequest : IRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Code { get; init; }
}
