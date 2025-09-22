using Application.Abstraction.Messaging;
using Application.Abstraction.Services;

namespace Application.Features.Brand.Commands.DeleteBrand.Notifications;

public class BrandDeletedImageDeleteHandler : INotificationHandler<BrandDeletedNotification>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IBackgroundJobClientWrapper _backgroundJobClientWrapper;

    public BrandDeletedImageDeleteHandler(
        IFileStorageService fileStorageService,
        IBackgroundJobClientWrapper backgroundJobClientWrapper)
    {
        _fileStorageService = fileStorageService;
        _backgroundJobClientWrapper = backgroundJobClientWrapper;
    }

    public Task Handle(BrandDeletedNotification notification, CancellationToken cancellationToken)
    {
        _backgroundJobClientWrapper.Enqueue(() => _fileStorageService.DeleteFileAsync(notification.brandImage, CancellationToken.None));
        return Task.CompletedTask;
    }
}
