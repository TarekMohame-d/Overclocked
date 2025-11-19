using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Product.Events;

public class ProductUpdatedEventHandler(IBackgroundJobClient jobClient, IFileStorageService fileStorageService)
    : IEventHandler<ProductUpdatedEvent>
{
    public Task HandleAsync(ProductUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => fileStorageService.DeleteFilesAsync(domainEvent.OldImages, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((ProductUpdatedEvent)domainEvent, cancellationToken);
}
