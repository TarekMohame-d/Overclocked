using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.CategoryAggregate.Events;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Category.Commands.EventHandlers;

public class CategoryDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<CategoryDeletedDeleteImageEventHandler> logger)
    : IDomainEventHandler<CategoryDeletedEvent>
{
    public Task Handle(CategoryDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Deleting category {CategoryId} image: {ImageUrl}",
            domainEvent.CategoryId,
            domainEvent.ImageUrl);

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));

        return Task.CompletedTask;
    }
}
