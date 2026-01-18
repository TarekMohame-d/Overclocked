using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.WishlistUseCases.DeleteWishlistItem;

public record DeleteWishlistItemRequest : IRequest<IEnumerable<WishlistItemResponse>>, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }
    public required Guid ProductId { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Wishlist(UserId.ToString())];
    public string? CacheSetKey => null;
}
