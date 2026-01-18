using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.Common.Shared.ValueObjects.Image;

public record Image : IValueObject
{
    public string Value { get; private init; } = default!;

    private Image() { }

    public static Result<Image> Create(string imageUrl)
    {
        Dictionary<string, string[]> validationErrors = ValidateImageUrl(imageUrl);

        if (validationErrors.Count != 0)
            return Result<Image>.ValidationFailure(validationErrors);

        return Result.Success(new Image { Value = imageUrl });
    }

    // For EF
    internal static Image Load(string value) => new() { Value = value };

    private static Dictionary<string, string[]> ValidateImageUrl(string imageUrl)
    {
        var validationKey = "Image";
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(imageUrl))
            errors.Add("Image URL is required.");

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uri))
            errors.Add("Invalid URL.");

        if (uri?.Scheme != Uri.UriSchemeHttp && uri?.Scheme != Uri.UriSchemeHttps)
            errors.Add("Invalid URL scheme.");

        if (!imageUrl.Contains("://res.cloudinary.com/over-clocked/"))
            errors.Add("Image URL must be hosted on res.cloudinary.com/over-clocked.");

        var extension = Path.GetExtension(uri?.LocalPath);
        string[] validExtensions = [".jpg", ".jpeg", ".png"];

        if (!string.IsNullOrWhiteSpace(extension) && !validExtensions.Contains(extension))
            errors.Add($"Invalid image extension, valid extensions are: {string.Join(", ", validExtensions)}.");

        return errors.Count > 0 ? new Dictionary<string, string[]> { { validationKey, errors.ToArray() } } : [];
    }
}
