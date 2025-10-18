using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Product.Commands.UpdateProduct.Notifications;

public class ProductUpdatedCacheInvalidationHandler : INotificationHandler<ProductUpdatedNotification>
{
    private readonly ICacheService _cache;

    public ProductUpdatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(ProductUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.Product(notification.Id.ToString()), cancellationToken);
        await _cache.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);
    }
}
