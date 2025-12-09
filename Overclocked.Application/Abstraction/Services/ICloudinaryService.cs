using Overclocked.Application.Common;

namespace Overclocked.Application.Abstraction.Services;

public interface ICloudinaryService
{
    CloudinarySignatureResponse GenerateUploadSignature(string category);
}
