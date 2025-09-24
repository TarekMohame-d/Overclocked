using System.Net;
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
        var account = new Account(
            settings.Value.CloudName,
            settings.Value.ApiKey,
            settings.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var publicId = ExtractPublicId(fileUrl);
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deletionParams = new DeletionParams(publicId)
        {
            PublicId = publicId,
            Invalidate = true
        };
        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.StatusCode == HttpStatusCode.OK)
            return true;

        throw new FileDeleteFailedException($"Image delete failed: {result.Error?.Message ?? "Unknown error"}");
    }

    public async Task<bool> DeleteFilesAsync(List<string> fileUrls, CancellationToken cancellationToken = default)
    {
        var publicIds = ExtractPublicIds(fileUrls);
        var delResParams = new DelResParams()
        {
            PublicIds = publicIds,
            Invalidate = true,
            All = true
        };
        var result = await _cloudinary.DeleteResourcesAsync(delResParams, cancellationToken);

        if (result.StatusCode == HttpStatusCode.OK)
            return true;

        throw new FileDeleteFailedException($"Image delete failed: {result.Error?.Message ?? "Unknown error"}");
    }

    private string? ExtractPublicId(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;

        // Split URL path into segments
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var publicIdStart = Array.FindIndex(segments, s => s.Equals("images", StringComparison.OrdinalIgnoreCase));
        if (publicIdStart < 0) return null;

        // Join rest into the public ID
        var publicIdWithExt = string.Join("/", segments.Skip(publicIdStart));

        // Remove extension if any
        var dotIndex = publicIdWithExt.LastIndexOf('.');
        return dotIndex > 0 ? publicIdWithExt.Substring(0, dotIndex) : publicIdWithExt;
    }

    private List<string> ExtractPublicIds(List<string> urls)
    {
        return urls
            .Select(ExtractPublicId)
            .OfType<string>()
            .ToList();
    }
}
