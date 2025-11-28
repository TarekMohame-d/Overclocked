using Application.Common.Results;
using Application.Services.Wishlist.DTOs.Request;
using Domain.Exceptions;

namespace Application.Services.Wishlist;

public sealed partial class WishlistService
{
    public async Task<Result> DeleteWishlistItemAsync(
        DeleteWishlistItemRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Wishlist wishlist =
            await wishlistRepository.SingleOrDefaultAsync(
                x => x.UserId == request.UserId,
                cancellationToken: cancellationToken)
            ?? throw new WishlistNotFoundException(request.UserId);

        await wishlistItemRepository.DeleteWhereAsync(
            x => x.Id == request.WishlistItemId && x.WishlistId == wishlist.Id,
            cancellationToken);

        return Result.Success();
    }
}
