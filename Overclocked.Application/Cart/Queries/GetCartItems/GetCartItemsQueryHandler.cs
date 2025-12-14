using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Cart.Mapping;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.ValueObjects;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Cart.Queries.GetCartItems;

public class GetCartItemsQueryHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository) : IQueryHandler<GetCartItemsQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(GetCartItemsQuery query, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(query.UserId);

        CartEntity cart = await cartRepository.GetAsync(userId, cancellationToken)
            ?? throw new CartNotFoundException(query.UserId);

        var productIds = cart.CartItems.Select(x => x.ProductId).ToList();
        List<ProductEntity> products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        CartResponse response = CartMapper.MapToResponse(cart, products);

        return Result.Success(response);
    }
}
