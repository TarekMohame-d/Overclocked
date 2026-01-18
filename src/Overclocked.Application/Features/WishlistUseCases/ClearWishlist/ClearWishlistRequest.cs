using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.WishlistUseCases.ClearWishlist;

public class ClearWishlistRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Wishlist(UserId.ToString())];
    public string? CacheSetKey => null;
}
