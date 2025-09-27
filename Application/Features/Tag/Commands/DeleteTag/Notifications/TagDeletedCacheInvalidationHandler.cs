using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Tag.Commands.DeleteTag.Notifications;

public class TagDeletedCacheInvalidationHandler : INotificationHandler<TagDeletedNotification>
{
    private readonly ICacheService _cache;

    public TagDeletedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task Handle(TagDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.Tag(notification.Id.ToString()), cancellationToken);
        await _cache.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
    }
}
