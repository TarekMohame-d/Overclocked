namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error CategoryNotFound = new(
        nameof(CategoryNotFound),
        ErrorType.NotFound,
        "Category not found.");

    public static readonly Error CategoryNameAlreadyExists = new(
        nameof(CategoryNameAlreadyExists),
        ErrorType.Conflict,
        "Category name already exists.");
}
