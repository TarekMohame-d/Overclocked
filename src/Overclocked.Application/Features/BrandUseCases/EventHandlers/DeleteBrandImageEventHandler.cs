using Hangfire;
using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.BrandAggregate.Events;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.BrandUseCases.EventHandlers;

public class BrandDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService,
    ILogger<BrandDeletedDeleteImageEventHandler> logger
) : IDomainEventHandler<BrandDeletedEvent>
{
    public Task Handle(BrandDeletedEvent domainEvent, CancellationToken ct = default)
    {
        logger.LogInformation("Deleting brand {BrandId} image: {ImageUrl}", domainEvent.BrandId, domainEvent.ImageUrl);

        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, ct));

        return Task.CompletedTask;
    }
}
