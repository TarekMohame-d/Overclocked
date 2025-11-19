namespace Domain.Exceptions;

public class CartNotFoundException : Exception
{
    public CartNotFoundException(Guid userId)
        : base($"The cart for user ID {userId} was not found, but was expected to exist.") { }

    public CartNotFoundException()
        : base() { }

    public CartNotFoundException(string? message)
        : base(message) { }

    public CartNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
