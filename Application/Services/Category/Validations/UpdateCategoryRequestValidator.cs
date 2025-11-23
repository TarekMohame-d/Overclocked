using Application.Services.Category.DTOs.Request;
using FluentValidation;

namespace Application.Services.Category.Validations;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
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
            .Must(url => url.StartsWith("https://res.cloudinary.com/over-clocked/", StringComparison.OrdinalIgnoreCase))
            .WithMessage("{PropertyName} must be hosted on res.cloudinary.com/over-clocked.");
    }

    private static bool ValidateImageExtension(string imageUrl)
    {
        string[] validExtensions = [".jpg", ".jpeg", ".png"];
        var extension = Path.GetExtension(imageUrl) ?? null;

        return extension is not null && validExtensions.Contains(extension);
    }
}
