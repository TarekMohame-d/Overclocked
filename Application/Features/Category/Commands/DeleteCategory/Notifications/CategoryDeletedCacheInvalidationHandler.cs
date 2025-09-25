using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Category.Commands.DeleteCategory.Notifications;

public class CategoryDeletedCacheInvalidationHandler : INotificationHandler<CategoryDeletedNotification>
{
    private readonly ICacheService _cache;

    public CategoryDeletedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task Handle(CategoryDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Category(notification.Id.ToString()), cancellationToken);
    }
}
