namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error BrandNotFound = new(nameof(BrandNotFound), ErrorType.NotFound, "Brand not found.");

    public static readonly Error BrandNameAlreadyExists = new(
        nameof(BrandNameAlreadyExists),
        ErrorType.Conflict,
        "Brand name already exists.");
}
