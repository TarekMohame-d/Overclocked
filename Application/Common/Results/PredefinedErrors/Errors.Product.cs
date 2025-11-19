namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error ProductNotFound = new(
        nameof(ProductNotFound),
        ErrorType.NotFound,
        "Product not found."
    );

    public static readonly Error ProductNameAlreadyExists = new(
        nameof(ProductNameAlreadyExists),
        ErrorType.Conflict,
        "Product name already exists."
    );
}
