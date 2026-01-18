using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.CategoryAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.CategoryUseCases.EventHandlers;

public class CategoryUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<CategoryUpdatedDeleteOldImageEventHandler> logger
) : IDomainEventHandler<CategoryImageUpdatedEvent>
{
    public Task Handle(CategoryImageUpdatedEvent domainEvent, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Updating category {CategoryId} Deleting old image: {ImageUrl}",
            domainEvent.CategoryId,
            domainEvent.ImageUrl
        );

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, ct));

        return Task.CompletedTask;
    }
}
