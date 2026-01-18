using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.CartUseCases.UpdateCartItem;

public record UpdateCartItemRequest : IRequest<CartResponse>, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }
    public required Guid CartItemId { get; init; }
    public required int Quantity { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Cart(UserId.ToString())];
    public string? CacheSetKey => null;
}
