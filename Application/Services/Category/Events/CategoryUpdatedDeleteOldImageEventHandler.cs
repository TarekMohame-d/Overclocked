using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Category.Events;

public class CategoryUpdatedDeleteOldImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService)
    : IEventHandler<CategoryUpdatedEvent>
{
    public Task HandleAsync(CategoryUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((CategoryUpdatedEvent)domainEvent, cancellationToken);
}
