using System.Data;
using FluentValidation;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ProductUseCases.CreateProduct;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.BrandId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("{PropertyName} is required.");

        RuleFor(x => x.CategoryId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("{PropertyName} is required.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(x => x.Thumbnail)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(ValidateImageUrl)
            .WithMessage(
                "{PropertyName} must be hosted on res.cloudinary.com/over-clocked and be a valid image file (jpg, jpeg, png)."
            );

        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");

        RuleFor(x => x.Price).GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");

        RuleFor(x => x.Discount)
            .InclusiveBetween(0m, 0.99m)
            .WithMessage("{PropertyName} must be between 0 and 0.99.")
            .When(x => x.Discount is not null);

        RuleFor(x => x.Tags).NotEmpty().WithMessage("At least one tag is required.");

        RuleFor(x => x.Specifications)
            .NotEmpty()
            .WithMessage("At least one tag is required.")
            .Must(ValidateSpecification)
            .WithMessage("Invalid specification.");

        RuleForEach(x => x.Images)
            .Must(ValidateImageUrl)
            .WithMessage(
                "{PropertyName} must be hosted on res.cloudinary.com/over-clocked and be a valid image file (jpg, jpeg, png)."
            )
            .When(x => x.Images?.Count > 0);
    }

    private static bool ValidateImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return false;

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri? uri))
            return false;

        if (uri?.Scheme != Uri.UriSchemeHttp && uri?.Scheme != Uri.UriSchemeHttps)
            return false;

        if (!imageUrl.Contains("://res.cloudinary.com/over-clocked/"))
            return false;

        var extension = Path.GetExtension(uri?.LocalPath);
        string[] validExtensions = [".jpg", ".jpeg", ".png"];

        return validExtensions.Contains(extension);
    }

    private static bool ValidateSpecification(List<ProductSpecificationDto> specificationDto)
    {
        foreach (ProductSpecificationDto spec in specificationDto)
        {
            if (string.IsNullOrWhiteSpace(spec.Name))
                return false;

            if (spec.Name.Length > 50)
                return false;

            if (string.IsNullOrWhiteSpace(spec.Value))
                return false;

            if (spec.Value.Length > 300)
                return false;
        }

        var hasDuplicateSpecs = specificationDto.GroupBy(x => x.Name.Trim().ToUpperInvariant()).Any(g => g.Count() > 1);

        return !hasDuplicateSpecs;
    }
}
