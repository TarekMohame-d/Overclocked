using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Domain.CategoryAggregate.Events;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Category.Commands.EventHandlers;

public class CategoryUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<CategoryUpdatedDeleteOldImageEventHandler> logger)
    : IDomainEventHandler<CategoryUpdatedEvent>
{
    public Task Handle(CategoryUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Updating category {CategoryId} Deleting old image: {ImageUrl}",
            domainEvent.CategoryId,
            domainEvent.ImageUrl);

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));

        return Task.CompletedTask;
    }
}
