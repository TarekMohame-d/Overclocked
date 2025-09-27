using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Tag.Commands.UpdateTag.Notifications;

public class TagUpdatedCacheInvalidationHandler : INotificationHandler<TagUpdatedNotification>
{
    private readonly ICacheService _cache;

    public TagUpdatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(TagUpdatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.Tag(notification.Id.ToString()), cancellationToken);
        await _cache.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
    }
}
