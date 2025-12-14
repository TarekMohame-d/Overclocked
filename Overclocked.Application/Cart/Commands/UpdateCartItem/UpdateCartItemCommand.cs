using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Contracts.Cart;

namespace Overclocked.Application.Cart.Commands.UpdateCartItem;

public record UpdateCartItemCommand : ICommand<CartResponse>, ICacheInvalidatorCommand
{
    public required Guid UserId { get; init; }
    public required Guid CartItemId { get; init; }
    public required int Quantity { get; init; }

    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Cart(UserId.ToString())
    ];

    public string? CacheSetKey => null;
}
