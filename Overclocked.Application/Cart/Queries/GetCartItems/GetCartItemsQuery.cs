using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Cart;

namespace Overclocked.Application.Cart.Queries.GetCartItems;

public record GetCartItemsQuery : IQuery<CartResponse>, ICachedQuery
{
    public required Guid UserId { get; init; }

    public string CacheKey => CacheKeys.Cart(UserId.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
