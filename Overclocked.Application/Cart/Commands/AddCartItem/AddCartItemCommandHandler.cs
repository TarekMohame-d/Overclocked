using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Cart.Mapping;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Cart.Commands.AddCartItem;

public class AddCartItemCommandHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    IProductRepository productRepository) : ICommandHandler<AddCartItemCommand, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var productId = ProductId.Create(command.ProductId);

        CartEntity cart = await cartRepository.GetCartAsync(userId, cancellationToken)
            ?? throw new CartNotFoundException(command.UserId);

        cart.AddCartItem(productId, command.Quantity);

        var productIds = cart.CartItems.Select(x => x.ProductId).ToList();
        List<ProductEntity> products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        CartResponse response = CartMapper.MapToResponse(cart, products);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(response);
    }
}
