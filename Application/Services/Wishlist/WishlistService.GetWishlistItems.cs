using Application.Common.Results;
using Application.Services.Wishlist.DTOs.Response;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Wishlist;

public sealed partial class WishlistService
{
    public async Task<Result<IEnumerable<WishlistItemResponse>>> GetWishlistItemsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Wishlist wishlist =
            await wishlistRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken: cancellationToken)
            ?? throw new WishlistNotFoundException(userId);

        List<WishlistItemResponse> wishlistItems = await wishlistItemRepository
            .Query()
            .Where(wi => wi.WishlistId == wishlist.Id)
            .Select(wi => new WishlistItemResponse
            {
                ProductId = wi.ProductId,
                ProductName = wi.Product!.Name,
                ProductDescription = wi.Product.Description,
                ProductPrice = wi.Product.Price,
                ProductThumbnail = wi.Product.Thumbnail
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<WishlistItemResponse>>.Success(wishlistItems);
    }
}
