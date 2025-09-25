using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Category.Commands.CreateCategory.Notifications;

public class CategoryCreatedCacheInvalidationHandler : INotificationHandler<CategoryCreatedNotification>
{
    private readonly ICacheService _cache;

    public CategoryCreatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(CategoryCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
    }
}
