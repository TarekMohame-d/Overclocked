using Application.Common.Results;
using Domain.Exceptions;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        Domain.Entities.Cart? cart =
            await cartRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken: cancellationToken)
            ?? throw new CartNotFoundException(userId);

        await cartItemRepository
            .DeleteWhereAsync(x => x.CartId == cart.Id, cancellationToken);

        return Result.Success();
    }
}
