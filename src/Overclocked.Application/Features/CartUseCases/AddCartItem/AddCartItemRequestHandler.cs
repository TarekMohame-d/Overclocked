using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Application.Features.CartUseCases.Mapping;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.CartUseCases.AddCartItem;

public class AddCartItemRequestHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    IProductReadRepository productRepository
) : IRequestHandler<AddCartItemRequest, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(AddCartItemRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);
        var productId = ProductId.Create(request.ProductId);

        if (!await productRepository.ExistsAsync(productId, ct))
            return Result.Failure<CartResponse>(ProductErrors.ProductNotFound(productId.Value));

        Cart cart = await cartRepository.GetAsync(userId, ct) ?? throw new CartNotFoundException(request.UserId);

        Result result = cart.AddCartItem(productId, request.Quantity);

        if (result.IsFailure)
            return Result.Failure<CartResponse>(result.Error);

        var productIds = cart.CartItems.Select(x => x.ProductId).ToList();
        List<Product> products = await productRepository.GetByIdsAsync(productIds, ct);

        CartResponse response = CartMapper.MapToResponse(cart, products);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(response);
    }
}
