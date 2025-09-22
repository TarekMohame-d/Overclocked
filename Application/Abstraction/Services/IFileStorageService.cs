namespace Application.Abstraction.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream stream, string fileName, string category, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}
