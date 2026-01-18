using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.UserUseCases.AddAddress;

public class AddAddressRequestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<AddAddressRequest>
{
    public async Task<Result> Handle(AddAddressRequest request, CancellationToken ct)
    {
        User? user = await userRepository.GetByIdAsync(UserId.Create(request.UserId), ct);

        if (user is null)
            return Result.Failure(UserErrors.NotFound(request.UserId));

        Result result = user.AddAddress(
            request.Apartment,
            request.Building,
            request.Street,
            request.City,
            request.PostalCode,
            request.Description
        );

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
