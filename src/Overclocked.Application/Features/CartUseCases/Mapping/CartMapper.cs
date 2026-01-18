using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.Entities;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.ProductAggregate;

namespace Overclocked.Application.Features.CartUseCases.Mapping;

public static class CartMapper
{
    public static CartResponse MapToResponse(Cart cart, List<Product> products)
    {
        var productMap = products.ToDictionary(p => p.Id);
        List<CartItemResponse> responseItems = [];

        foreach (CartItem item in cart.CartItems)
        {
            Product product = productMap[item.ProductId];

            Money finalPrice = product.CalculateFinalPrice();
            var lineTotal = Math.Round(finalPrice.Value * item.Quantity, 2, MidpointRounding.ToEven);

            responseItems.Add(
                new CartItemResponse
                {
                    CartItemId = item.Id.Value,
                    ProductId = item.ProductId.Value,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    ProductThumbnail = product.Thumbnail.Value,
                    UnitPrice = product.Price.Value,
                    Quantity = item.Quantity,
                    Discount = product.Discount.Value,
                    LineTotal = lineTotal,
                }
            );
        }

        return new CartResponse
        {
            CartItems = responseItems,
            Total = Math.Round(responseItems.Sum(ci => ci.LineTotal), 2, MidpointRounding.ToEven),
        };
    }
}
