using Api.Common.Routing;

namespace Api.Routing;

public class CloudinarySignatureRoute : BaseRoute
{
    public const string Prefix = $"{Base}/cloudinary-signature";

    public const string Generate = $"{Prefix}/generate";
}
