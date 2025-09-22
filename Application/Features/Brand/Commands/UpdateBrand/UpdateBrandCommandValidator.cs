using Application.Common.Validators;
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
            .NotEmpty()
            .WithMessage("{PropertyName} is required and must not be empty or whitespace.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");

        // Ensure at least one is provided
        RuleFor(x => x.ImageFile)
            .NotNull()
            .When(x => string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("Either ImageFile or ImageUrl must be provided.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .When(x => x.ImageFile is null)
            .WithMessage("Either ImageFile or ImageUrl must be provided.");

        // Ensure only one is provided
        RuleFor(x => x.ImageFile)
            .Must((cmd, file) => file is null || string.IsNullOrWhiteSpace(cmd.ImageUrl))
            .WithMessage("Only one of ImageUrl or ImageFile can be provided.");

        // Validate ImageUrl if present
        RuleFor(x => x.ImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("{PropertyName} must be a valid URL.")
            .Must(url => url!.StartsWith("https://res.cloudinary.com/over-clocked/", StringComparison.OrdinalIgnoreCase))
            .WithMessage("{PropertyName} must be hosted on res.cloudinary.com/over-clocked.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        // Validate ImageFile if present
        RuleFor(x => x.ImageFile!)
            .ValidateImageFile()
            .When(x => x.ImageFile is not null);
    }
}
