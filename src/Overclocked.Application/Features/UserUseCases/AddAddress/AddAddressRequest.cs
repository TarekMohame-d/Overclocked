using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.UserUseCases.AddAddress;

public record AddAddressRequest : IRequest
{
    public required Guid UserId { get; init; }
    public required int Apartment { get; init; }
    public required string Building { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Description { get; init; }
}
