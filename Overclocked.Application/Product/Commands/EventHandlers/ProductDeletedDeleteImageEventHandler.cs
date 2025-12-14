using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.Events;

namespace Overclocked.Application.Product.Commands.EventHandlers;

public class ProductDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<ProductDeletedDeleteImageEventHandler> logger)
    : IDomainEventHandler<ProductDeletedEvent>
{
    public Task Handle(ProductDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting Product {ProductId} images", domainEvent.ProductId);

        jobClient.Enqueue(() => fileStorageService.DeleteFilesAsync(domainEvent.ImagesUrls, cancellationToken));

        return Task.CompletedTask;
    }
}
