using Overclocked.Contracts.Wishlist;
using Overclocked.Domain.WishlistAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;
using WishlistEntity = Overclocked.Domain.WishlistAggregate.Wishlist;

namespace Overclocked.Application.Wishlist.Mapping;

public static class WishlistMapper
{
    public static WishlistResponse MapToResponse(WishlistEntity wishlist, IEnumerable<ProductEntity> products)
    {
        var productMap = products.ToDictionary(p => p.Id);
        var responseItems = new List<WishlistResponse.WishlistItemResponse>();

        foreach(WishlistItem item in wishlist.WishlistItems)
        {
            ProductEntity product = productMap[item.ProductId];

            responseItems.Add(new WishlistResponse.WishlistItemResponse
            {
                ProductId = item.ProductId.Value,
                ProductName = product.Name,
                ProductDescription = product.Description,
                ProductThumbnail = product.Thumbnail,
            });
        }

        return new WishlistResponse
        {
            WishlistItems = responseItems
        };
    }
}
