using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Brand.Events;

public class BrandUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService)
    : IEventHandler<BrandUpdatedEvent>
{
    public Task HandleAsync(BrandUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((BrandUpdatedEvent)domainEvent, cancellationToken);
}
