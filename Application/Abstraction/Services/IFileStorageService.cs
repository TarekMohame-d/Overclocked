namespace Application.Abstraction.Services;

public interface IFileStorageService
{
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task DeleteFilesAsync(IEnumerable<string> fileUrls, CancellationToken cancellationToken = default);
}
