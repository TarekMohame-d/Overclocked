using Application.Common.Results;
using Domain.Exceptions;

namespace Application.Services.Wishlist;

public sealed partial class WishlistService
{
    public async Task<Result> ClearWishlistAsync(Guid userId, CancellationToken cancellationToken)
    {
        Domain.Entities.Wishlist wishlist =
            await wishlistRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken: cancellationToken)
            ?? throw new WishlistNotFoundException(userId);

        await wishlistItemRepository
            .DeleteWhereAsync(x => x.WishlistId == wishlist.Id, cancellationToken);

        return Result.Success();
    }
}
