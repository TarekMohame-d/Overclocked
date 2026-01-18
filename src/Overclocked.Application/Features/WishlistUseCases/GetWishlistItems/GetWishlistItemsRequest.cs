using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.WishlistUseCases.GetWishlistItems;

public record GetWishlistItemsRequest : IRequest<IEnumerable<WishlistItemResponse>>, ICachedRequest
{
    public required Guid UserId { get; init; }

    public string CacheKey => CacheKeys.Wishlist(UserId.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
