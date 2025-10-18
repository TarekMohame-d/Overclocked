using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Product.Commands.DeleteProduct.Notifications;

public class ProductDeletedCacheInvalidationHandler : INotificationHandler<ProductDeletedNotification>
{
    private readonly ICacheService _cache;

    public ProductDeletedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task Handle(ProductDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.Product(notification.Id.ToString()), cancellationToken);
        await _cache.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
    }
}
