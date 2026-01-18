using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.BrandAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.BrandUseCases.EventHandlers;

public class BrandUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<BrandUpdatedDeleteOldImageEventHandler> logger
) : IDomainEventHandler<BrandImageUpdatedEvent>
{
    public Task Handle(BrandImageUpdatedEvent domainEvent, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Updating brand {BrandId} Deleting old image: {ImageUrl}",
            domainEvent.BrandId,
            domainEvent.ImageUrl
        );

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, ct));

        return Task.CompletedTask;
    }
}
