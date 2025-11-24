namespace Domain.Exceptions;

public class InvalidCartItemQuantityException : Exception
{
    public InvalidCartItemQuantityException(Guid productId, int attemptedQty, int maxQty)
        : base($"Attempted to add quantity {attemptedQty} for product {productId}, but stock limit is {maxQty}.") { }

    public InvalidCartItemQuantityException()
        : base() { }

    public InvalidCartItemQuantityException(string? message)
        : base(message) { }

    public InvalidCartItemQuantityException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
