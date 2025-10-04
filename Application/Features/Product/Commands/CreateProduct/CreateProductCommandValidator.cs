using Application.Abstraction.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext _context;
    public CreateProductCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.BrandId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MustAsync(async (id, cancellation) =>
            {
                return await _context.Brands.FindAsync(id) is not null;
            })
            .WithMessage("{PropertyName}: {PropertyValue} does not exist.");

        RuleFor(x => x.CategoryId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MustAsync(async (id, cancellation) =>
            {
                return await _context.Categories.FindAsync(id) is not null;
            })
            .WithMessage("{PropertyName}: {PropertyValue} does not exist.");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
            .MustAsync(async (name, cancellation) =>
            {
                return !await _context.Products.AnyAsync(p => p.NormalizedName == name.ToUpper(), cancellation);
            })
            .WithMessage("A product with the same name already exists.");

        RuleFor(x => x.Thumbnail)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            .WithMessage("{PropertyName} must be a valid HTTP/HTTPS URL.")
            .Must(url => url.StartsWith("https://res.cloudinary.com/over-clocked/", StringComparison.OrdinalIgnoreCase))
            .WithMessage("{PropertyName} must be hosted on res.cloudinary.com/over-clocked.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");

        RuleFor(x => x.Discount)
            .InclusiveBetween(0m, 0.99m).WithMessage("{PropertyName} must be between 0 and 0.99.");

        ValidateTags();

        ValidateImages();

        ValidateSpecification();
    }

    private void ValidateTags()
    {
        RuleFor(x => x.Tags)
            .CustomAsync(async (tags, context, cancellation) =>
            {
                if (tags is null || !tags.Any())
                {
                    context.AddFailure("Tags", "At least one tag is required.");
                    return;
                }

                var existingIds = await _context.Tags
                    .Where(t => tags.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync(cancellation);

                var missingTags = tags.Except(existingIds).ToList();

                if (missingTags.Any())
                {
                    context.AddFailure("Tags",
                        $"The following tags do not exist: {string.Join(", ", missingTags)}");
                }
            });
    }

    private void ValidateImages()
    {
        RuleFor(x => x.Images)
            .Custom((images, context) =>
            {
                var invalidUrls = images!
                    .Where(url => !Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                    .ToList();

                var invalidHosts = images!
                    .Where(url => !url.StartsWith("https://res.cloudinary.com/over-clocked/",
                                                StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (invalidHosts.Any() || invalidUrls.Any())
                {
                    context.AddFailure("Images", "Image URLs must be valid HTTP/HTTPS URLs hosted on res.cloudinary.com/over-clocked.");
                }
            }).When(x => x.Images is not null && x.Images!.Any());
    }

    private void ValidateSpecification()
    {
        RuleFor(x => x.Specification)
            .Custom((specs, context) =>
            {
                if (specs == null || !specs.Any())
                {
                    context.AddFailure("Specification", "At least one specification is required.");
                    return;
                }

                // Empty or too long names
                var invalidNames = specs
                    .Where(s => string.IsNullOrWhiteSpace(s.Name) || s.Name.Length > 50)
                    .ToList();

                // Empty or too long values
                var invalidValues = specs
                    .Where(s => string.IsNullOrWhiteSpace(s.Value) || s.Value.Length > 300)
                    .ToList();

                if (invalidNames.Any() || invalidValues.Any())
                {
                    context.AddFailure("Specification",
                        "Name and Value are required and must not exceed 50 and 300 characters respectively.");
                }

                var duplicateNames = specs
                    .GroupBy(s => s.Name.Trim().ToUpper())
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateNames.Any())
                {
                    context.AddFailure("Specification",
                        "Name must be unique. Duplicate names: " + string.Join(", ", duplicateNames));
                }
            });
    }
}
