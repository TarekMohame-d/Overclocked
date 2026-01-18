namespace Overclocked.Domain.PaymentAggregate.ValueObjects;

public enum PaymentStatus
{
    Pending = 1, // Payment is initiated but not yet completed or confirmed (e.g., waiting for online payment or COD).
    Paid, // Payment completed successfully and confirmed.
    Failed, // Payment attempt failed (e.g., card declined, timeout).
    Refunded, // Payment was returned to the customer after cancellation or return.
    Cancelled, // Payment was cancelled by the user or system before processing.
}
