using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Product.Events;

public class ProductDeletedEventHandler(IBackgroundJobClient jobClient, IFileStorageService fileStorageService)
    : IEventHandler<ProductDeletedEvent>
{
    public Task HandleAsync(ProductDeletedEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => fileStorageService.DeleteFilesAsync(domainEvent.OldImages, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((ProductDeletedEvent)domainEvent, cancellationToken);
}
