namespace Application.Services;

public interface IFileStorageService
{
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> DeleteFilesAsync(List<string> fileUrls, CancellationToken cancellationToken = default);
}
