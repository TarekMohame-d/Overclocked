using Application.Common.Results;
using Application.Services.Cart.DTOs.Response;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result<CartItemResponse>> GetCartItemsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Cart cart =
            await cartRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken: cancellationToken)
            ?? throw new CartNotFoundException(userId);

        List<CartItemResponse.CartItem> cartItems = await cartItemRepository
            .Query()
            .Where(ci => ci.CartId == cart.Id)
            .Select(ci => new CartItemResponse.CartItem
            {
                CartItemId = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product!.Name,
                ProductDescription = ci.Product.Description,
                ProductThumbnail = ci.Product.Thumbnail,
                UnitPrice = ci.Product.Price,
                Quantity = ci.Quantity,
                Discount = ci.Product.Discount,
                LineTotal = Math.Round(ci.Product.Price * (1m - ci.Product.Discount) * ci.Quantity, 2),
            })
            .ToListAsync(cancellationToken);

        var response = new CartItemResponse
        {
            CartItems = cartItems,
            Total = Math.Round(cartItems.Sum(ci => ci.LineTotal), 2),
        };

        return Result<CartItemResponse>.Success(response);
    }
}
