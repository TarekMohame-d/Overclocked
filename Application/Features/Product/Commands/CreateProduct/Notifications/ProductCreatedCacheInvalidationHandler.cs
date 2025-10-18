using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Product.Commands.CreateProduct.Notifications;

public class ProductCreatedCacheInvalidationHandler : INotificationHandler<ProductCreatedNotification>
{
    private readonly ICacheService _cache;

    public ProductCreatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);
    }
}
