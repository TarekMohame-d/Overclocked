using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Contracts.Wishlist;

namespace Overclocked.Application.Wishlist.Commands.AddWishlistItem;

public record AddWishlistItemCommand : ICommand<WishlistResponse>, ICacheInvalidatorCommand
{
    public required Guid UserId { get; init; }
    public required Guid ProductId { get; init; }

    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Wishlist(UserId.ToString())
    ];

    public string? CacheSetKey => null;
}
