namespace Overclocked.SharedKernel.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(Guid orderId)
        : base($"The Order for order ID {orderId} was not found.") { }

    public OrderNotFoundException() { }

    public OrderNotFoundException(string? message)
        : base(message) { }

    public OrderNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
