using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.ProductAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.ProductUseCases.EventHandlers;

public class ProductUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<ProductUpdatedDeleteOldImageEventHandler> logger
) : IDomainEventHandler<ProductImagesRemovedEvent>
{
    public Task Handle(ProductImagesRemovedEvent domainEvent, CancellationToken ct = default)
    {
        logger.LogInformation("Updating product {ProductId} Deleting old images", domainEvent.ProductId);

        jobClient.Enqueue(() => fileStorageService.DeleteFilesAsync(domainEvent.ImagesUrls, ct));

        return Task.CompletedTask;
    }
}
