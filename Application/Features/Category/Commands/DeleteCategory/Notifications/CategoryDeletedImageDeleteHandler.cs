using Application.Abstraction.Messaging;
using Application.Abstraction.Services;

namespace Application.Features.Category.Commands.DeleteCategory.Notifications;

public class CategoryDeletedImageDeleteHandler : INotificationHandler<CategoryDeletedNotification>
{
    private readonly IFileStorageService _fileStorageService;

    public CategoryDeletedImageDeleteHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(CategoryDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _fileStorageService.DeleteFileAsync(notification.ImageUrl, cancellationToken);
    }
}
