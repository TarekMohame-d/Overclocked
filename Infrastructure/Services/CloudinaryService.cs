using Application.Abstraction.Services;
using Application.Features.CloudinarySignature;
using CloudinaryDotNet;
using Domain.Configurations;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinarySettings _settings;

    public CloudinaryService(IOptions<CloudinarySettings> cloudinaryOptions)
    {
        var settings = cloudinaryOptions.Value;

        _settings = settings;

        var account = new Account(
            _settings.CloudName,
            _settings.ApiKey,
            _settings.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public CloudinarySignatureResponse GenerateUploadSignature(string category)
    {
        // The timestamp is crucial for the signature's validity period.
        var timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        var transformation = new Transformation()
            .Quality("auto")
            .FetchFormat("auto")
            .Width(800)
            .Crop("limit")
            .Generate(); // .Generate() creates the string "c_limit,f_auto,q_auto,w_800"

        string folder = $"images/{category}";

        var parametersToSign = new SortedDictionary<string, object>
        {
            { "timestamp", timestamp },
            { "folder", folder },
            { "transformation", transformation }
        };

        string signature = _cloudinary.Api.SignParameters(parametersToSign);

        // 4. Return the response DTO as usual.
        return new CloudinarySignatureResponse
        {
            Signature = signature,
            Timestamp = timestamp,
            ApiKey = _settings.ApiKey,
            CloudName = _settings.CloudName,
            Folder = folder,
            Transformation = transformation
        };
    }
}
