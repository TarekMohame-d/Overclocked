using Application.Common.Results;
using Application.Services.Cart.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result<IEnumerable<CartItemResponse>>> GetCartItemsAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        List<CartItemResponse> cartItems = await cartRepository
            .Query()
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .SelectMany(c => c.CartItems)
            .Select(ci => new CartItemResponse
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product!.Name,
                UnitPrice = ci.Product.Price,
                Quantity = ci.Quantity,
                Discount = ci.Product.Discount,
                LineTotal = ci.Product.Price * (1m - ci.Product.Discount) * ci.Quantity,
            })
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<CartItemResponse>>.Success(cartItems);
    }
}
