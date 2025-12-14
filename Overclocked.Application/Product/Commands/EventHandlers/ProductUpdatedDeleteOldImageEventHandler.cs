using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.Events;

namespace Overclocked.Application.Product.Commands.EventHandlers;

public class ProductUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<ProductUpdatedDeleteOldImageEventHandler> logger)
    : IDomainEventHandler<ProductImagesRemovedEvent>
{
    public Task Handle(ProductImagesRemovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating product {ProductId} Deleting old images", domainEvent.ProductId);

        jobClient.Enqueue(() => fileStorageService.DeleteFilesAsync(domainEvent.ImagesUrls, cancellationToken));

        return Task.CompletedTask;
    }
}
