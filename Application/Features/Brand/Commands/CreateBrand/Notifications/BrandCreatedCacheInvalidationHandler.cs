using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Brand.Commands.CreateBrand.Notifications;

public class BrandCreatedCacheInvalidationHandler : INotificationHandler<BrandCreatedNotification>
{
    private readonly ICacheService _cache;

    public BrandCreatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(BrandCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
    }
}
