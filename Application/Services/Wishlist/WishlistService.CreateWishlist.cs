using Application.Common.Results;

namespace Application.Services.Wishlist;

public sealed partial class WishlistService
{
    public async Task<Result> CreateWishlistAsync(Guid userId, CancellationToken cancellationToken)
    {
        var wishlist = new Domain.Entities.Wishlist() { UserId = userId };

        await wishlistRepository.AddAsync(wishlist, cancellationToken);

        return Result.Success();
    }
}
