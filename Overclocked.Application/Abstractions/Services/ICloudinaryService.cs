using Overclocked.Application.Common;

namespace Overclocked.Application.Abstractions.Services;

public interface ICloudinaryService
{
    CloudinarySignatureResponse GenerateUploadSignature(string category);
}
