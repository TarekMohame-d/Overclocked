namespace Domain.StaticData;

public enum PaymentStatusType
{
    Pending = 1,        // Payment is initiated but not yet completed or confirmed.
    Paid,               // Payment completed successfully and confirmed.
    Failed,             // Payment attempt failed (e.g., card declined, timeout).
    Refunded,           // Payment was returned to the customer after cancellation or return.
    PartiallyRefunded,  // Only part of the payment was refunded (e.g., one item returned).
    Cancelled           // Payment was cancelled by the user or system before processing.
}
