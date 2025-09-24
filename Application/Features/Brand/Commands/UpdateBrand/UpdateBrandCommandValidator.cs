using FluentValidation;

namespace Application.Features.Brand.Commands.UpdateBrand;

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandWithIdCommand>
{
    public UpdateBrandCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .Must(BeAValidImageExtension).WithMessage("{PropertyName} must be a valid image file (jpg, jpeg, png).");

        RuleFor(x => x.ImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("{PropertyName} must be a valid URL.")
            .Must(url => url.StartsWith("https://res.cloudinary.com/over-clocked/", StringComparison.OrdinalIgnoreCase))
            .WithMessage("{PropertyName} must be hosted on res.cloudinary.com/over-clocked.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }

    private bool BeAValidImageExtension(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return false;

        string[] validExtensions = { ".jpg", ".jpeg", ".png" };
        string extension = Path.GetExtension(imageUrl).ToLowerInvariant();

        return validExtensions.Contains(extension);
    }
}
