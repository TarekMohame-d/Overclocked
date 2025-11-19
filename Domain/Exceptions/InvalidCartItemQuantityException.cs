namespace Domain.Exceptions;

public class InvalidCartItemQuantityException : Exception
{
    public InvalidCartItemQuantityException(Guid productId, int attemptedQty, int maxQty)
        : base($"Attempted to add quantity {attemptedQty} for product {productId}, but stock limit is {maxQty}.") { }
}
