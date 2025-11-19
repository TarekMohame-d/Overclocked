namespace Domain.StaticData;

public enum RefundStatusType
{
    Pending = 1, // Refund is initiated but not yet completed or confirmed.
    Refunded, // Refund completed successfully and confirmed.
    CanNotBeRefunded, // Refund is not allowed for this order (e.g., Refund time expired).
    Failed, // Refund attempt failed (e.g., card declined, timeout).
    Cancelled, // Refund was cancelled by the user or system before processing.
}
