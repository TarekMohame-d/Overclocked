using Overclocked.SharedKernel;

namespace Overclocked.Domain.CategoryAggregate;

public static class CategoryErrors
{
    public static Error CategoryNotFound(Guid id) =>
        Error.NotFound("Category.NotFound", $"The category with ID '{id}' was not found.");

    public static Error CategoryNameAlreadyExists => Error.Conflict("Category.NameAlreadyExists", "Name already exists.");

    public static Error CategoryNameIsRequired => Error.Validation("Category.NameIsRequired", "Name is required.");

    public static Error CategoryNameIsTooLong =>
        Error.Validation("Category.NameIsTooLong", "Name must be less than 50 characters.");
}
