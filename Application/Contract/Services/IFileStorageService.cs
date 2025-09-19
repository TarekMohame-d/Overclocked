using Microsoft.AspNetCore.Http;

namespace Application.Contract.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string category, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}
