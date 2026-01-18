using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.UserUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.UserUseCases.GetAddresses;

public record GetAddressesRequest : IRequest<IEnumerable<AddressResponse>>
{
    public required Guid UserId { get; init; }
}
