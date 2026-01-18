using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.AuthenticationUseCases.Register;

public record RegisterRequest : IRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
}
