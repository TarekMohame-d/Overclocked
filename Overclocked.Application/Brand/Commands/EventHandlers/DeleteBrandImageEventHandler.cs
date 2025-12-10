using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Domain.BrandAggregate.Events;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Brand.Commands.EventHandlers;

public class BrandDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<BrandDeletedDeleteImageEventHandler> logger)
    : IDomainEventHandler<BrandDeletedEvent>
{
    public Task Handle(BrandDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting brand {BrandId} image: {ImageUrl}", domainEvent.BrandId, domainEvent.ImageUrl);

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));

        return Task.CompletedTask;
    }
}
