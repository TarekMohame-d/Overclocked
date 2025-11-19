using Application.Common.Results;
using Domain.Exceptions;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        Domain.Entities.Cart? cart =
            await cartRepository.GetCartWithItemsAsync(userId, cancellationToken)
            ?? throw new CartNotFoundException(userId);

        cart.CLearCart();

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
