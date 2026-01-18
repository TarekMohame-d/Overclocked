using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.CartUseCases.GetCartItems;

public record GetCartItemsRequest : IRequest<CartResponse>, ICachedRequest
{
    public required Guid UserId { get; init; }

    public string CacheKey => CacheKeys.Cart(UserId.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
