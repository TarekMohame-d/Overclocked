using Application.Abstraction.DomainServices;
using Application.Services.CloudinarySignature;
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
        CloudinarySettings settings = cloudinaryOptions.Value;

        _settings = settings;

        var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public CloudinarySignatureResponse GenerateUploadSignature(string category)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var transformation = new Transformation()
            .Quality("auto")
            .FetchFormat("auto")
            .Width(800)
            .Crop("limit")
            .Generate(); // .Generate() creates the string "c_limit,f_auto,q_auto,w_800"

        var folder = $"images/{category}";

        var parametersToSign = new SortedDictionary<string, object>
        {
            { "timestamp", timestamp },
            { "folder", folder },
            { "transformation", transformation },
        };

        var signature = _cloudinary.Api.SignParameters(parametersToSign);

        return new CloudinarySignatureResponse
        {
            Signature = signature,
            Timestamp = timestamp,
            ApiKey = _settings.ApiKey,
            CloudName = _settings.CloudName,
            Folder = folder,
            Transformation = transformation,
        };
    }
}
