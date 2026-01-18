using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Application.Features.CartUseCases.Mapping;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.CartUseCases.GetCartItems;

public class GetCartItemsRequestHandler(ICartReadRepository cartRepository, IProductReadRepository productRepository)
    : IRequestHandler<GetCartItemsRequest, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(GetCartItemsRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);

        Cart cart = await cartRepository.GetAsync(userId, ct) ?? throw new CartNotFoundException(request.UserId);

        var productIds = cart.CartItems.Select(x => x.ProductId).ToList();
        List<Product> products = await productRepository.GetByIdsAsync(productIds, ct);

        CartResponse response = CartMapper.MapToResponse(cart, products);

        return Result.Success(response);
    }
}
