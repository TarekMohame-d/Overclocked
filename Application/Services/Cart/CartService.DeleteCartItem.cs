using Application.Common.Results;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> DeleteCartItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        Domain.Entities.Cart cart =
            await cartRepository.GetCartWithItemsAsync(userId, cancellationToken: cancellationToken)
            ?? throw new CartNotFoundException(userId);

        cart.RemoveItem(productId);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
