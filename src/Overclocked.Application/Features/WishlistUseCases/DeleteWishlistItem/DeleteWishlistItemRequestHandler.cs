using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Application.Features.WishlistUseCases.Mapping;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.WishlistUseCases.DeleteWishlistItem;

public class DeleteWishlistItemRequestHandler(
    IWishlistRepository wishlistRepository,
    IProductReadRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteWishlistItemRequest, IEnumerable<WishlistItemResponse>>
{
    public async Task<Result<IEnumerable<WishlistItemResponse>>> Handle(DeleteWishlistItemRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);
        var productId = ProductId.Create(request.ProductId);

        Wishlist wishlist = await wishlistRepository.GetAsync(userId, ct) ?? throw new WishlistNotFoundException(request.UserId);

        wishlist.RemoveWishlistItem(productId);

        var productIds = wishlist.WishlistItems.Select(x => x.ProductId).ToList();
        List<Product> products = await productRepository.GetByIdsAsync(productIds, ct);

        IEnumerable<WishlistItemResponse> response = WishlistMapper.MapToResponse(wishlist, products);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(response);
    }
}
