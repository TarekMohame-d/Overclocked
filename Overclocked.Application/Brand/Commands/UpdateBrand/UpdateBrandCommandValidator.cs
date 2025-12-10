using FluentValidation;

namespace Overclocked.Application.Brand.Commands.UpdateBrand;

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .Must(ValidateImageExtension)
            .WithMessage("{PropertyName} must be a valid image file (jpg, jpeg, png).")
            .Must(url =>
                Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("{PropertyName} must be a valid URL.")
            .Must(url => url.Contains("://res.cloudinary.com/over-clocked/"))
            .WithMessage("{PropertyName} must be hosted on res.cloudinary.com/over-clocked.");
    }

    private static bool ValidateImageExtension(string imageUrl)
    {
        if(!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uri))
        {
            return false;
        }

        var path = uri.LocalPath;
        var extension = Path.GetExtension(path);

        string[] validExtensions = [".jpg", ".jpeg", ".png"];

        return !string.IsNullOrEmpty(extension) && validExtensions.Contains(extension.ToLowerInvariant());
    }
}
