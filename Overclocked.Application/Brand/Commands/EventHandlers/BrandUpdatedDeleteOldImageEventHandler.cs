using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Domain.BrandAggregate.Events;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Brand.Commands.EventHandlers;

public class BrandUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<BrandUpdatedDeleteOldImageEventHandler> logger)
    : IDomainEventHandler<BrandUpdatedEvent>
{
    public Task Handle(BrandUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Updating brand {BrandId} Deleting old image: {ImageUrl}",
            domainEvent.BrandId,
            domainEvent.ImageUrl);

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));

        return Task.CompletedTask;
    }
}
