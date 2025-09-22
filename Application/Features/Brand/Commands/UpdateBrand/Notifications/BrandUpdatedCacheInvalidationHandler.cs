using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Brand.Commands.UpdateBrand.Notifications;

public class BrandUpdatedCacheInvalidationHandler : INotificationHandler<BrandUpdatedNotification>
{
    private readonly ICacheService _cache;

    public BrandUpdatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(BrandUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Brand(notification.id.ToString()), cancellationToken);
    }
}
