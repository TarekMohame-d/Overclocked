using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.ProductAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.ProductUseCases.EventHandlers;

public class ProductDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<ProductDeletedDeleteImageEventHandler> logger
) : IDomainEventHandler<ProductDeletedEvent>
{
    public Task Handle(ProductDeletedEvent domainEvent, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting Product {ProductId} images", domainEvent.ProductId);

        jobClient.Enqueue(() => fileStorageService.DeleteFilesAsync(domainEvent.ImagesUrls, ct));

        return Task.CompletedTask;
    }
}
