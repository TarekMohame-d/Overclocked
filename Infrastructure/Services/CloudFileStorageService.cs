using System.Net;
using Application.Abstraction.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Configurations;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class CloudFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudFileStorageService(IOptions<CloudinarySettings> settings)
    {
        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string category, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var guidFileName = Guid.NewGuid().ToString();
        var fileExtension = Path.GetExtension(fileName);

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription($"{guidFileName}{fileExtension}", stream),
            PublicId = guidFileName, // use custom PublicId
            Folder = $"images/{category}",
            UseFilename = false,
            UniqueFilename = false,
            Overwrite = true,
            Transformation = new Transformation()
            .Quality("auto")
            .FetchFormat("auto")
            .Width(800)
            .Crop("limit")
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.StatusCode == HttpStatusCode.OK)
            return result.SecureUrl.ToString();

        throw new FileUploadFailedException($"Image upload failed: {result.Error?.Message ?? "Unknown error"}");
    }

    public async Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var publicId = ExtractPublicIdFromUrl(fileUrl);
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deletionParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.Result is "ok" or "not_found")
            return true;

        throw new FileDeleteFailedException($"Image delete failed: {result.Error?.Message ?? "Unknown error"}");
    }

    private string? ExtractPublicIdFromUrl(string url)
    {
        string[] segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 3) return null;

        // Get last Three segments
        var lastThree = segments[^3..];

        // Remove extension from last segment
        lastThree[^1] = Path.GetFileNameWithoutExtension(lastThree[^1]);

        return string.Join("/", lastThree);
    }
}
