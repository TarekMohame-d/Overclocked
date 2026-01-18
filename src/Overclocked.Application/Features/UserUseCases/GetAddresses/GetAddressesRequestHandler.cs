using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.UserUseCases.DTOs.Responses;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.UserUseCases.GetAddresses;

public class GetAddressesRequestHandler(IUserReadRepository userRepository)
    : IRequestHandler<GetAddressesRequest, IEnumerable<AddressResponse>>
{
    public async Task<Result<IEnumerable<AddressResponse>>> Handle(GetAddressesRequest request, CancellationToken ct)
    {
        User? user = await userRepository.GetByIdAsync(UserId.Create(request.UserId), ct);

        if (user is null)
            return Result.Failure<IEnumerable<AddressResponse>>(UserErrors.NotFound(request.UserId));

        IEnumerable<AddressResponse> addresses = user.Addresses.Select(a => new AddressResponse(
            a.Street,
            a.City,
            a.PostalCode,
            a.Description
        ));

        return Result.Success(addresses);
    }
}
