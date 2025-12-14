using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Contracts.Wishlist;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.ValueObjects;
using WishlistEntity = Overclocked.Domain.WishlistAggregate.Wishlist;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;
using Overclocked.Application.Wishlist.Mapping;

namespace Overclocked.Application.Wishlist.Queries.GetWishlistItems;

public class GetWishlistItemsQueryHandler(
    IWishlistRepository wishlistRepository,
    IProductRepository productRepository) : IQueryHandler<GetWishlistItemsQuery, WishlistResponse>
{
    public async Task<Result<WishlistResponse>> Handle(GetWishlistItemsQuery query, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(query.UserId);

        WishlistEntity wishlist = await wishlistRepository.GetAsync(userId, cancellationToken)
            ?? throw new WishlistNotFoundException(query.UserId);

        var productIds = wishlist.WishlistItems.Select(x => x.ProductId).ToList();
        List<ProductEntity> products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        WishlistResponse response = WishlistMapper.MapToResponse(wishlist, products);

        return Result.Success(response);
    }
}
