namespace Overclocked.Api.Routing;

public abstract class CloudinarySignatureRoute : BaseRoute
{
    private const string Prefix = $"{Base}/cloudinary";

    public const string UploadSignature = $"{Prefix}/upload-signature";
}
