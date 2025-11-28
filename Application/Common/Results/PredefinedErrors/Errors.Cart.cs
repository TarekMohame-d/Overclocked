namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error InvalidCartItemQuantity = new(
        nameof(InvalidCartItemQuantity),
        ErrorType.BadRequest,
        "Cannot add item. The total quantity exceeds available stock.");

    public static readonly Error CartItemNotFound = new(
        nameof(CartItemNotFound),
        ErrorType.BadRequest,
        "The cart item was not found.");
}
