using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Hangfire;

namespace Application.Services.Category.Events;

public class CategoryDeletedDeleteImageEventHandler(
    IBackgroundJobClient jobClient,
    IFileStorageService fileStorageService)
    : IEventHandler<CategoryDeletedEvent>
{
    public Task HandleAsync(CategoryDeletedEvent domainEvent, CancellationToken cancellationToken)
    {
        jobClient.Enqueue(() => fileStorageService.DeleteFileAsync(domainEvent.ImageUrl, cancellationToken));
        return Task.CompletedTask;
    }

    public Task Handle(IDomainEvent domainEvent, CancellationToken cancellationToken) =>
        HandleAsync((CategoryDeletedEvent)domainEvent, cancellationToken);
}
