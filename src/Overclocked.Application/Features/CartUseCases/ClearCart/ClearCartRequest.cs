using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.CartUseCases.ClearCart;

public record ClearCartRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Cart(UserId.ToString())];
    public string? CacheSetKey => null;
}
