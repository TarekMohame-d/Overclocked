using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.AuthenticationUseCases.ForgetPassword;

public record ForgetPasswordRequest : IRequest
{
    public required string Email { get; init; }
}
