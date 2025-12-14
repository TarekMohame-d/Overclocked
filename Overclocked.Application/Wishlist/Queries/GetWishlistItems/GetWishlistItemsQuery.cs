using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Wishlist;

namespace Overclocked.Application.Wishlist.Queries.GetWishlistItems;

public record GetWishlistItemsQuery : IQuery<WishlistResponse>, ICachedQuery
{
    public required Guid UserId { get; init; }

    public string CacheKey => CacheKeys.Wishlist(UserId.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
