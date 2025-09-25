using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Category.Commands.UpdateCategory.Notifications;

public class CategoryUpdatedCacheInvalidationHandler : INotificationHandler<CategoryUpdatedNotification>
{
    private readonly ICacheService _cache;

    public CategoryUpdatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(CategoryUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
        await _cache.RemoveAsync(CacheKeys.Category(notification.id.ToString()), cancellationToken);
    }
}
