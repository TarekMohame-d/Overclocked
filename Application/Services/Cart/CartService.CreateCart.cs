using Application.Common.Results;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> CreateCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = new Domain.Entities.Cart() { UserId = userId };

        await cartRepository.AddAsync(cart, cancellationToken);

        return Result.Success();
    }
}
