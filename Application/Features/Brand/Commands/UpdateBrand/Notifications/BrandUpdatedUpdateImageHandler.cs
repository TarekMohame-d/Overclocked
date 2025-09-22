using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Brand.Commands.UpdateBrand.Notifications;

public class BrandUpdatedUpdateImageHandler : INotificationHandler<BrandUpdatedNotification>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobClientWrapper _backgroundJobClient;

    public BrandUpdatedUpdateImageHandler(
        IFileStorageService fileStorageService,
        IBrandRepository brandRepository,
        IUnitOfWork unitOfWork,
        IBackgroundJobClientWrapper backgroundJobClient)
    {
        _fileStorageService = fileStorageService;
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task Handle(BrandUpdatedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.image is not null)
        {
            var tempPath = await SaveTempFileAsync(notification.image, cancellationToken);

            var uploadJobId = _backgroundJobClient.Enqueue(() =>
                UploadAndUpdateBrandImageAsync(notification.id, tempPath, notification.image.FileName));

            _backgroundJobClient.ContinueJobWith(uploadJobId, () =>
                DeleteBrandImageAsync(notification.imageUrl!));
        }
    }

    public async Task UploadAndUpdateBrandImageAsync(Guid brandId, string filePath, string fileName)
    {
        // upload the file from the temp location
        await using var stream = File.OpenRead(filePath);
        var imageUrl = await _fileStorageService.UploadFileAsync(stream, fileName, "brands");

        // update the brand entity
        var brand = await _brandRepository.GetByIdAsync([brandId]);
        if (brand is not null)
        {
            brand.Image = imageUrl;
            _brandRepository.Update(brand);
            await _unitOfWork.CompleteAsync();
        }

        File.Delete(filePath); // cleanup
    }

    public async Task DeleteBrandImageAsync(string imageUrl)
    {
        await _fileStorageService.DeleteFileAsync(imageUrl);
    }

    public async Task<string> SaveTempFileAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{file.FileName}");
        await using var stream = new FileStream(tempPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);
        return tempPath;
    }
}
