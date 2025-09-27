using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;

namespace Application.Features.Tag.Commands.CreateTag.Notifications;

public class TagCreatedCacheInvalidationHandler : INotificationHandler<TagCreatedNotification>
{
    private readonly ICacheService _cache;

    public TagCreatedCacheInvalidationHandler(ICacheService cache)
    {
        _cache = cache;
    }
    public async Task Handle(TagCreatedNotification notification, CancellationToken cancellationToken)
    {
        await _cache.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
    }
}
