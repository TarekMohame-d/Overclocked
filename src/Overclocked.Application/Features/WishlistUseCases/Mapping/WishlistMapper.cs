using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Application.Features.WishlistUseCases.Mapping;

public static class WishlistMapper
{
    public static IEnumerable<WishlistItemResponse> MapToResponse(Wishlist wishlist, List<Product> products)
    {
        var productMap = products.ToDictionary(p => p.Id);
        List<WishlistItemResponse> responseItems = [];

        foreach (WishlistItem item in wishlist.WishlistItems)
        {
            Product product = productMap[item.ProductId];

            responseItems.Add(
                new WishlistItemResponse
                {
                    ProductId = item.ProductId.Value,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    ProductThumbnail = product.Thumbnail.Value,
                }
            );
        }

        return responseItems;
    }
}
