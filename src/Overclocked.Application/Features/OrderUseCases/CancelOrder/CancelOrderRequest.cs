using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.OrderUseCases.CancelOrder;

public record CancelOrderRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }
    public required Guid OrderId { get; init; }
    public required bool RefundToWallet { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.OrderSet(UserId.ToString());
}
