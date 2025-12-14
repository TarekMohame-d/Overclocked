using Overclocked.Contracts.Cart;
using Overclocked.Domain.CartAggregate.Entities;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Cart.Mapping;

public static class CartMapper
{
    public static CartResponse MapToResponse(CartEntity cart, IEnumerable<ProductEntity> products)
    {
        var productMap = products.ToDictionary(p => p.Id);
        var responseItems = new List<CartResponse.CartItemResponse>();

        foreach(CartItem item in cart.CartItems)
        {
            ProductEntity product = productMap[item.ProductId];

            var unitPrice = product.Price.Amount;
            var discount = product.Discount.Amount;
            var lineTotal = Math.Round(unitPrice * (1m - discount) * item.Quantity, 2);

            responseItems.Add(new CartResponse.CartItemResponse
            {
                CartItemId = item.Id.Value,
                ProductId = item.ProductId.Value,
                ProductName = product.Name,
                ProductDescription = product.Description,
                ProductThumbnail = product.Thumbnail,
                UnitPrice = unitPrice,
                Quantity = item.Quantity,
                Discount = discount,
                LineTotal = lineTotal
            });
        }

        return new CartResponse
        {
            CartItems = responseItems,
            Total = Math.Round(responseItems.Sum(ci => ci.LineTotal), 2),
        };
    }
}
