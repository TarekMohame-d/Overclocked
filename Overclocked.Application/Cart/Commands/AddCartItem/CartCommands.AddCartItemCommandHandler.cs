using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Application.Cart.Mapping;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Cart.Commands;

public sealed partial class CartCommands
{
    public async Task<Result<CartItemResponse>> AddCartItemCommandHandler(
        AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var productId = ProductId.Create(command.ProductId);

        CartEntity cart = await cartRepository.GetCartAsync(userId, cancellationToken)
            ?? throw new CartNotFoundException(command.UserId);

        cart.AddCartItem(productId, command.Quantity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var productIds = cart.CartItems.Select(x => x.ProductId).ToList();
        List<ProductEntity> products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        CartItemResponse response = CartMapper.MapToResponse(cart, products);

        return Result<CartItemResponse>.Success(response);
    }
}
