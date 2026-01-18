using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.AuthenticationUseCases.ResendEmailConfirmationCode;

public record ResendEmailConfirmationCodeRequest : IRequest
{
    public required string Email { get; init; }
}
