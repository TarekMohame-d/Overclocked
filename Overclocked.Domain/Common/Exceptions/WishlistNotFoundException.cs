namespace Overclocked.Domain.Common.Exceptions;

public class WishlistNotFoundException : Exception
{
    public WishlistNotFoundException(Guid userId)
        : base($"The wishlist for user ID {userId} was not found, but was expected to exist.") { }

    public WishlistNotFoundException()
        : base() { }

    public WishlistNotFoundException(string? message)
        : base(message) { }

    public WishlistNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
