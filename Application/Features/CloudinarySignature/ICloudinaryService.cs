using Application.Features.CloudinarySignature;

namespace Application.Interfaces;

public interface ICloudinaryService
{
    /// <summary>
    /// Generates a secure, time-limited signature for a direct client-side upload to Cloudinary.
    /// </summary>
    /// <returns>A DTO containing the signature and necessary parameters for the client.</returns>
    CloudinarySignatureResponse GenerateUploadSignature(string category);
}
