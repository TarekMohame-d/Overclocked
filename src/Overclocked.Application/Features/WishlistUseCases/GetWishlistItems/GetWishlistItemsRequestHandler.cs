using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Application.Features.WishlistUseCases.Mapping;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.WishlistUseCases.GetWishlistItems;

public class GetWishlistItemsRequestHandler(IWishlistReadRepository wishlistRepository, IProductReadRepository productRepository)
    : IRequestHandler<GetWishlistItemsRequest, IEnumerable<WishlistItemResponse>>
{
    public async Task<Result<IEnumerable<WishlistItemResponse>>> Handle(GetWishlistItemsRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);

        Wishlist wishlist = await wishlistRepository.GetAsync(userId, ct) ?? throw new WishlistNotFoundException(request.UserId);

        var productIds = wishlist.WishlistItems.Select(x => x.ProductId).ToList();
        List<Product> products = await productRepository.GetByIdsAsync(productIds, ct);

        IEnumerable<WishlistItemResponse> response = WishlistMapper.MapToResponse(wishlist, products);

        return Result.Success(response);
    }
}
