using Domain.Repositories;
using FluentValidation;

namespace Application.Features.Brand.Commands.CreateBrand;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    private readonly IBrandRepository _brandRepository;
    public CreateBrandCommandValidator(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellation) =>
            {
                bool exists = await _brandRepository
                    .AnyAsync(x => x.NormalizedName == name.ToUpper(), cancellation);
                return !exists;
            })
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("{PropertyName} already exists.");

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
