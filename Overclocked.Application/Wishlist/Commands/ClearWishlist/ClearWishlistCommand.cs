using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Wishlist.Commands.ClearWishlist;

public class ClearWishlistCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid UserId { get; init; }
    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Wishlist(UserId.ToString())
    ];

    public string? CacheSetKey => null;
}
