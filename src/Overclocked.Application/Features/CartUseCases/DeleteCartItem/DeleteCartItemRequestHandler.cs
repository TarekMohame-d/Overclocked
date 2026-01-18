using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Application.Features.CartUseCases.Mapping;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.CartUseCases.DeleteCartItem;

public class DeleteCartItemRequestHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    IProductReadRepository productRepository
) : IRequestHandler<DeleteCartItemRequest, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(DeleteCartItemRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);
        var carItemId = CartItemId.Create(request.CartItemId);

        Cart cart = await cartRepository.GetAsync(userId, ct) ?? throw new CartNotFoundException(request.UserId);

        cart.RemoveCartItem(carItemId);

        var productIds = cart.CartItems.Select(x => x.ProductId).ToList();
        List<Product> products = await productRepository.GetByIdsAsync(productIds, ct);

        CartResponse response = CartMapper.MapToResponse(cart, products);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(response);
    }
}
