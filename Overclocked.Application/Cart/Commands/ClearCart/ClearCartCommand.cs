using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Cart.Commands.ClearCart;

public record ClearCartCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid UserId { get; init; }
    public string[] CacheKeys =>
    [
        Common.Constants.CacheKeys.Cart(UserId.ToString())
    ];

    public string? CacheSetKey => null;
}
