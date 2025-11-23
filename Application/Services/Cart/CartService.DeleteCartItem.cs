using Application.Common.Results;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> DeleteCartItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        Domain.Entities.Cart cart =
            await cartRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken: cancellationToken)
            ?? throw new CartNotFoundException(userId);

        await cartItemRepository.DeleteWhereAsync(
            x => x.ProductId == productId && x.CartId == cart.Id,
            cancellationToken);

        return Result.Success();
    }
}
