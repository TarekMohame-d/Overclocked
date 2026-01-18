using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.CategoryAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.CategoryUseCases.EventHandlers;

public class CategoryDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<CategoryDeletedDeleteImageEventHandler> logger
) : IDomainEventHandler<CategoryDeletedEvent>
{
    public Task Handle(CategoryDeletedEvent domainEvent, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting category {CategoryId} image: {ImageUrl}", domainEvent.CategoryId, domainEvent.ImageUrl);

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, ct));

        return Task.CompletedTask;
    }
}
