using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class CategoryErrors
{
    public static Error CategoryNotFound(Guid id)
    {
        return new(nameof(CategoryNotFound), ErrorType.NotFound, $"The category with ID '{id}' was not found.");
    }

    public static readonly Error CategoryNameAlreadyExists = new(
        nameof(CategoryNameAlreadyExists),
        ErrorType.Conflict,
        "Category name already exists.");
}
