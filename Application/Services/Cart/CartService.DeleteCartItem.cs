using Application.Common.Results;
using Application.Services.Cart.DTOs.Request;
using Domain.Exceptions;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> DeleteCartItemAsync(DeleteCartItemRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Cart cart =
            await cartRepository.SingleOrDefaultAsync(
                x => x.UserId == request.UserId,
                cancellationToken: cancellationToken)
            ?? throw new CartNotFoundException(request.UserId);

        await cartItemRepository.DeleteWhereAsync(
            x => x.Id == request.CartItemId && x.CartId == cart.Id,
            cancellationToken);

        return Result.Success();
    }
}
