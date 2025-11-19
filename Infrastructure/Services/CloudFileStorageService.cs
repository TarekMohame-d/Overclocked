using Application.Abstraction.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Configurations;
using Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class CloudFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudFileStorageService(IOptions<CloudinarySettings> settings)
    {
        var account = new Account(settings.Value.CloudName, settings.Value.ApiKey, settings.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var publicId = ExtractPublicId(fileUrl);
        if(string.IsNullOrEmpty(publicId))
        {
            throw new FileDeleteFailedException($"Invalid public Id for file: {fileUrl}");
        }

        var deletionParams = new DeletionParams(publicId) { PublicId = publicId, Invalidate = true };

        await _cloudinary.DestroyAsync(deletionParams);
    }

    public async Task DeleteFilesAsync(IEnumerable<string> fileUrls, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> publicIds = ExtractPublicIds(fileUrls);
        var delResParams = new DelResParams
        {
            PublicIds = publicIds.ToList(),
            Invalidate = true,
            All = true,
        };

        await _cloudinary.DeleteResourcesAsync(delResParams, cancellationToken);
    }

    private static string? ExtractPublicId(string url)
    {
        // Split URL path into segments
        var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var publicIdStart = Array.FindIndex(segments, s => s.Equals("images", StringComparison.OrdinalIgnoreCase));
        if(publicIdStart < 0)
            return null;

        // Join rest into the public ID
        var publicIdWithExt = string.Join("/", segments.Skip(publicIdStart));

        // Remove extension if any
        var dotIndex = publicIdWithExt.LastIndexOf('.');
        return dotIndex > 0 ? publicIdWithExt[..dotIndex] : publicIdWithExt;
    }

    private static IEnumerable<string> ExtractPublicIds(IEnumerable<string> urls) =>
        urls.Select(ExtractPublicId).OfType<string>();
}
