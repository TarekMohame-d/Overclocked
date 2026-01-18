using Overclocked.SharedKernel;

namespace Overclocked.Domain.ProductAggregate;

public static class ProductErrors
{
    public static Error ProductNotFound(Guid id) =>
        Error.NotFound("Product.NotFound", $"The product with ID '{id}' was not found.");

    public static Error ProductNameAlreadyExists => Error.Conflict("Product.NameAlreadyExists", "Name already exists.");

    public static Error NotEnoughStock = Error.BadRequest("Product.NotEnoughStock", "Not enough stock.");

    public static Error ProductNameIsRequired => Error.Validation("Product.Name", "Name is required.");

    public static Error ProductNameIsTooLong => Error.Validation("Product.Name", "Name must be less than 50 characters.");

    public static Error ProductDescriptionIsRequired => Error.Validation("Product.Description", "Description is required.");

    public static Error ProductDescriptionIsTooLong =>
        Error.Validation("Product.Description", "Description must be less than 500 characters.");

    public static Error SpecificationNameNotUnique =>
        Error.Validation("Product.Specifications", "Specification Name must be unique.");

    public static Error SpecificationNameIsRequired => Error.Validation("Specification.Name", "Specification Name is required.");

    public static Error SpecificationNameIsTooLong =>
        Error.Validation("Specification.Name", "Specification Name must be less than 50 characters.");

    public static Error SpecificationValueIsRequired =>
        Error.Validation("Specification.Value", "Specification Value is required.");

    public static Error SpecificationValueIsTooLong =>
        Error.Validation("Specification.Value", "Specification Value must be less than 50 characters.");

    public static Error DuplicateTags => Error.Validation("Product.Tags", "Tags must be unique.");

    public static Error EmptyTags => Error.Validation("Product.Tags", "At least one tag is required.");

    public static Error InvalidRatingTotalScore =>
        Error.Validation("ProductRating.TotalScore", "Total score cannot be negative.");

    public static Error InvalidReviewRating =>
        Error.Validation("ProductRating.ReviewRating", "Review rating must be between 1 and 5.");

    public static Error InvalidRatingReviewCount =>
        Error.Validation("ProductRating.ReviewCount", "Review count cannot be negative.");

    public static Error ProductStockIsInvalid => Error.Validation("Product.Stock", "Stock cannot be negative.");
}
