using Application.Abstraction.Messaging;
using Application.Abstraction.Services;

namespace Application.Features.Brand.Commands.DeleteBrand.Notifications;

public class BrandDeletedImageDeleteHandler : INotificationHandler<BrandDeletedNotification>
{
    private readonly IFileStorageService _fileStorageService;

    public BrandDeletedImageDeleteHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(BrandDeletedNotification notification, CancellationToken cancellationToken)
    {
        await _fileStorageService.DeleteFileAsync(notification.ImageUrl, cancellationToken);
    }
}
