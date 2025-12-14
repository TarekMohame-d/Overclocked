using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class CategoryErrors
{
    public static Error CategoryNotFound(Guid id)
    {
        return new("Category.NotFound", $"The category with ID '{id}' was not found.", ErrorType.NotFound);
    }

    public static readonly Error CategoryNameAlreadyExists = new(
        "Category.NameAlreadyExists",
        "Category name already exists.",
        ErrorType.Conflict);
}
