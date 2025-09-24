namespace Application.Features.CloudinarySignature;

public sealed class CloudinarySignatureResponse
{
    /// <summary>
    /// The secure, server-generated signature for the upload request.
    /// </summary>
    public required string Signature { get; init; }

    /// <summary>
    /// The timestamp (in Unix time) used to generate the signature.
    /// /// </summary>
    public required long Timestamp { get; init; }

    /// <summary>
    /// The public API key for your Cloudinary account.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// The name of your Cloudinary cloud.
    /// </summary>
    public required string CloudName { get; init; }

    /// <summary>
    /// The folder where you want to upload the file.
    /// </summary>
    public required string Folder { get; init; }

    /// <summary>
    /// The transformation string for the upload request.
    /// </summary>
    public required string Transformation { get; init; }
}
