using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Wishlist.DTOs.Request;
using Domain.Exceptions;

namespace Application.Services.Wishlist;

public sealed partial class WishlistService
{
    public async Task<Result> AddWishlistItemAsync(
        Guid userId,
        AddWishlistItemRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Wishlist wishlist =
            await wishlistRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                asNoTracking: false,
                cancellationToken)
            ?? throw new WishlistNotFoundException(userId);

        var productExists = await productRepository
            .AnyAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);

        if(!productExists)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        var itemExists = await wishlistItemRepository
            .AnyAsync(item => item.WishlistId == wishlist.Id && item.ProductId == request.ProductId, cancellationToken);

        if(!itemExists)
        {
            wishlist.AddWishlistItem(request.ProductId);

            await unitOfWork.CompleteAsync(cancellationToken);
        }

        return Result.Success();
    }
}
