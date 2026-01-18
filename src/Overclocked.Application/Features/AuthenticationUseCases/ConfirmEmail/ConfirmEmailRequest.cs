using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.AuthenticationUseCases.ConfirmEmail;

public record ConfirmEmailRequest : IRequest
{
    public required string Email { get; init; }
    public required string Code { get; init; }
}
